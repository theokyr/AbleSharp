using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using AbleSharp.Lib;
using AbleSharp.SDK;
using ReactiveUI;
using Avalonia.Media;
using Avalonia.ReactiveUI;

namespace AbleSharp.GUI.ViewModels.Tools;

public class MergeProjectsViewModel : ReactiveObject
{
    private readonly ILogger<MergeProjectsViewModel> _logger;
    private string _outputFilePath = string.Empty;
    private string _mergeSettingExample = "Keep All";
    private bool _isMerging;
    private double _mergeProgress;
    private bool _isMergeProgressIndeterminate;
    private string _statusMessage = string.Empty;
    private IBrush _statusMessageColor;
    private bool _isDragOver;

    public ObservableCollection<string> SelectedProjects { get; set; } = new();

    public string OutputFilePath
    {
        get => _outputFilePath;
        set => this.RaiseAndSetIfChanged(ref _outputFilePath, value);
    }

    public string MergeSettingExample
    {
        get => _mergeSettingExample;
        set => this.RaiseAndSetIfChanged(ref _mergeSettingExample, value);
    }

    public bool IsMerging
    {
        get => _isMerging;
        private set => this.RaiseAndSetIfChanged(ref _isMerging, value);
    }

    public double MergeProgress
    {
        get => _mergeProgress;
        private set => this.RaiseAndSetIfChanged(ref _mergeProgress, value);
    }

    public bool IsMergeProgressIndeterminate
    {
        get => _isMergeProgressIndeterminate;
        private set => this.RaiseAndSetIfChanged(ref _isMergeProgressIndeterminate, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public IBrush StatusMessageColor
    {
        get => _statusMessageColor;
        private set => this.RaiseAndSetIfChanged(ref _statusMessageColor, value);
    }

    /// <summary>
    /// Indicates whether a drag operation is currently over the drop zone.
    /// Used to provide visual feedback in the UI.
    /// </summary>
    public bool IsDragOver
    {
        get => _isDragOver;
        set => this.RaiseAndSetIfChanged(ref _isDragOver, value);
    }

    public ReactiveCommand<Unit, Unit> AddProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectOutputFileCommand { get; }
    public ReactiveCommand<Unit, Unit> MergeProjectsCommand { get; }

    public MergeProjectsViewModel()
    {
        _logger = LoggerService.GetLogger<MergeProjectsViewModel>();
        _statusMessageColor = new SolidColorBrush(Color.Parse("#FFFFFF"));

        // Initialize Commands with AvaloniaScheduler.Instance to ensure UI thread execution
        AddProjectsCommand = ReactiveCommand.CreateFromTask(AddProjectsAsync, outputScheduler: AvaloniaScheduler.Instance);

        RemoveSelectedProjectsCommand = ReactiveCommand.Create(
            RemoveSelectedProjects,
            this.WhenAnyValue(vm => vm.SelectedProjects.Count, count => count > 0),
            AvaloniaScheduler.Instance
        );

        SelectOutputFileCommand = ReactiveCommand.CreateFromTask(SelectOutputFileAsync, outputScheduler: AvaloniaScheduler.Instance);

        var canMerge = this.WhenAnyValue(
            vm => vm.SelectedProjects.Count,
            vm => vm.OutputFilePath,
            vm => vm.IsMerging,
            (count, outputFilePath, isMerging) => count >= 2 && !string.IsNullOrEmpty(outputFilePath) && !isMerging
        );

        MergeProjectsCommand = ReactiveCommand.CreateFromTask(
            MergeProjectsAsync,
            canMerge,
            AvaloniaScheduler.Instance
        );
    }

    public void AddProject(string projectPath)
    {
        if (!SelectedProjects.Contains(projectPath))
        {
            SelectedProjects.Add(projectPath);
            _logger.LogInformation($"Added project: {projectPath}");
        }
    }

    private async Task AddProjectsAsync()
    {
        _logger.LogInformation("Adding projects to merge");

        var filePaths = await FileDialogService.ShowOpenFilesDialogAsync();

        if (filePaths != null)
            foreach (var path in filePaths)
                AddProject(path);
    }

    private void RemoveSelectedProjects()
    {
        if (SelectedProjects.Count > 0)
        {
            var projectPath = SelectedProjects[SelectedProjects.Count - 1];
            SelectedProjects.Remove(projectPath);
            _logger.LogInformation($"Removed project: {projectPath}");
        }
    }

    private async Task SelectOutputFileAsync()
    {
        _logger.LogInformation("Selecting output file for merged project");

        var filePath = await FileDialogService.ShowSaveFileDialogAsync("MergedProject.als");

        if (!string.IsNullOrEmpty(filePath))
        {
            OutputFilePath = filePath;
            _logger.LogInformation($"Selected output file: {filePath}");
        }
    }

    private async Task MergeProjectsAsync()
    {
        if (SelectedProjects.Count < 2)
        {
            ShowError("At least two projects are required to perform a merge.");
            return;
        }

        if (string.IsNullOrEmpty(OutputFilePath))
        {
            ShowError("Output file path is not specified.");
            return;
        }

        try
        {
            IsMerging = true;
            IsMergeProgressIndeterminate = true;
            ShowStatus("Starting merge process...", "#FFFFFF");

            var projects = new List<AbletonProject>();
            var totalSteps = SelectedProjects.Count + 2; // Loading + Merging + Saving
            var currentStep = 0;

            // Load all projects
            foreach (var path in SelectedProjects)
                try
                {
                    ShowStatus($"Loading: {Path.GetFileName(path)}", "#FFFFFF");
                    var project = await Task.Run(() => AbletonProjectHandler.LoadFromFile(path));

                    if (project != null)
                    {
                        projects.Add(project);
                        currentStep++;
                        MergeProgress = (double)currentStep / totalSteps * 100;
                        _logger.LogInformation($"Successfully loaded: {path}");
                    }
                    else
                    {
                        ShowError($"Failed to load project: {path}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error loading {Path.GetFileName(path)}: {ex.Message}");
                    return;
                }

            if (projects.Count < 2)
            {
                ShowError("Not enough projects loaded successfully to perform merge.");
                return;
            }

            // Merge projects
            ShowStatus("Merging projects...", "#FFFFFF");
            IsMergeProgressIndeterminate = true;
            var mergedProject = await Task.Run(() => AbletonProjectMerger.MergeProjects(projects));
            currentStep++;
            MergeProgress = (double)currentStep / totalSteps * 100;

            if (mergedProject == null)
            {
                ShowError("Failed to merge projects.");
                return;
            }

            // Save merged project
            ShowStatus("Saving merged project...", "#FFFFFF");
            await Task.Run(() => AbletonProjectHandler.SaveToFile(mergedProject, OutputFilePath));
            currentStep++;
            MergeProgress = 100;

            // Show success message
            ShowSuccess("Projects merged successfully!");

            // Clear selection after successful merge
            SelectedProjects.Clear();
            OutputFilePath = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during project merge");
            ShowError($"Error during merge: {ex.Message}");
        }
        finally
        {
            IsMerging = false;
            IsMergeProgressIndeterminate = false;
        }
    }

    private void ShowStatus(string message, string colorHex)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse(colorHex));
        _logger.LogInformation(message);
    }

    private void ShowError(string message)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse("#FF4444"));
        _logger.LogError(message);
    }

    private void ShowSuccess(string message)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse("#44FF44"));
        _logger.LogInformation(message);
    }
}