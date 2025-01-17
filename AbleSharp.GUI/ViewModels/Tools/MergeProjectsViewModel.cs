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
using AbleSharp.SDK.Options;
using ReactiveUI;
using Avalonia.Media;
using Avalonia.ReactiveUI;

namespace AbleSharp.GUI.ViewModels.Tools;

public class MergeProjectsViewModel : ReactiveObject
{
    private readonly ILogger<MergeProjectsViewModel> _logger;
    private readonly AbleSharpSdk _sdk = AbleSharpSdk.Instance;
    private string _outputFilePath = string.Empty;
    private string _mergeSettingExample = "Keep All";
    private bool _isMerging;
    private double _mergeProgress;
    private bool _isMergeProgressIndeterminate;
    private string _statusMessage = string.Empty;
    private IBrush _statusMessageColor;
    private bool _isDragOver;
    private bool _showStatusMessage;

    public ObservableCollection<string> SelectedProjects { get; } = new();

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

    public bool ShowStatusMessage
    {
        get => _showStatusMessage;
        private set => this.RaiseAndSetIfChanged(ref _showStatusMessage, value);
    }

    public bool IsDragOver
    {
        get => _isDragOver;
        set => this.RaiseAndSetIfChanged(ref _isDragOver, value);
    }

    public bool HasFiles => SelectedProjects.Count > 0;

    public ReactiveCommand<Unit, Unit> AddProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedProjectsCommand { get; }
    public ReactiveCommand<string, Unit> RemoveProjectCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectOutputFileCommand { get; }
    public ReactiveCommand<Unit, Unit> MergeProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearStatusCommand { get; }

    public MergeProjectsViewModel()
    {
        _logger = LoggerService.GetLogger<MergeProjectsViewModel>();
        _statusMessageColor = new SolidColorBrush(Color.Parse("#FFFFFF"));

        // Initialize Commands
        AddProjectsCommand = ReactiveCommand.CreateFromTask(AddProjectsAsync, outputScheduler: AvaloniaScheduler.Instance);

        RemoveSelectedProjectsCommand = ReactiveCommand.Create(
            RemoveSelectedProjects,
            this.WhenAnyValue(vm => vm.SelectedProjects.Count, count => count > 0),
            AvaloniaScheduler.Instance
        );

        RemoveProjectCommand = ReactiveCommand.Create<string>(
            RemoveProject,
            outputScheduler: AvaloniaScheduler.Instance
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

        ClearStatusCommand = ReactiveCommand.Create(
            () =>
            {
                ShowStatusMessage = false;
                return Unit.Default;
            },
            outputScheduler: AvaloniaScheduler.Instance
        );

        SelectedProjects.CollectionChanged += (s, e) =>
        {
            _logger.LogDebug($"[MergeProjectsViewModel] SelectedProjects updated: {string.Join(", ", SelectedProjects)}");
            this.RaisePropertyChanged(nameof(HasFiles));
        };
    }

    public void AddProjects(IEnumerable<string> projectPaths)
    {
        foreach (var path in projectPaths)
        {
            if (!path.EndsWith(".als", StringComparison.OrdinalIgnoreCase)) continue;

            var normalizedPath = NormalizePath(path);
            if (SelectedProjects.Contains(normalizedPath)) continue;

            SelectedProjects.Add(normalizedPath);
            _logger.LogInformation($"Added project: {normalizedPath}");
        }
    }

    private async Task AddProjectsAsync()
    {
        _logger.LogDebug($"[MergeProjectsWindow] AddProjectsAsync ViewModel instance: {GetHashCode()}");
        _logger.LogInformation("[MergeProjectsViewModel] Adding projects to merge");

        var filePaths = await FileDialogService.ShowOpenFilesDialogAsync();

        if (filePaths == null) return;

        AddProjects(filePaths);
    }

    private void RemoveSelectedProjects()
    {
        var selectedProjects = SelectedProjects.ToList();
        foreach (var project in selectedProjects) RemoveProject(project);
    }

    private void RemoveProject(string projectPath)
    {
        if (SelectedProjects.Contains(projectPath))
        {
            SelectedProjects.Remove(projectPath);
            _logger.LogInformation($"[MergeProjectsViewModel] Removed project: {projectPath}");
        }
    }

    private async Task SelectOutputFileAsync()
    {
        _logger.LogInformation("[MergeProjectsViewModel] Selecting output file for merged project");

        var filePath = await FileDialogService.ShowSaveFileDialogAsync("MergedProject.als");

        if (!string.IsNullOrEmpty(filePath))
        {
            OutputFilePath = filePath;
            _logger.LogInformation($"[MergeProjectsViewModel] Selected output file: {filePath}");
        }
    }

    private void ShowError(string message)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse("#FF4444"));
        ShowStatusMessage = true;
        _logger.LogError(message);
    }

    private void ShowSuccess(string message)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse("#44FF44"));
        ShowStatusMessage = true;
        _logger.LogInformation(message);
    }

    private void UpdateProgress(string message)
    {
        StatusMessage = message;
        StatusMessageColor = new SolidColorBrush(Color.Parse("#FFFFFF"));
        ShowStatusMessage = true;
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
            UpdateProgress("Starting merge process...");

            var projects = new List<AbletonProject>();
            var totalSteps = SelectedProjects.Count + 2; // Loading + Merging + Saving
            var currentStep = 0;

            // Load all projects
            foreach (var path in SelectedProjects)
                try
                {
                    UpdateProgress($"Loading: {Path.GetFileName(path)}");

                    var project = _sdk.OpenProject(path, new ProjectOpenOptions()
                    {
                        Logger = msg => _logger.LogInformation(msg)
                    });

                    if (project != null)
                    {
                        projects.Add(project);
                        currentStep++;
                        MergeProgress = (double)currentStep / totalSteps * 100;
                        _logger.LogInformation($"[MergeProjectsViewModel] Successfully loaded: {path}");
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
            UpdateProgress("Merging projects...");
            IsMergeProgressIndeterminate = true;

            var mergedProject = _sdk.MergeProjects(projects, new ProjectMergeOptions
            {
                Logger = msg => _logger.LogInformation(msg),
                NamingConflicts = ConflictResolution.Rename,
                PreserveColors = true,
                MergeScenes = true
            });

            currentStep++;
            MergeProgress = (double)currentStep / totalSteps * 100;

            if (mergedProject == null)
            {
                ShowError("Failed to merge projects.");
                return;
            }

            // Save merged project
            UpdateProgress("Saving merged project...");

            _sdk.SaveProject(mergedProject, OutputFilePath, new ProjectSaveOptions
            {
                Logger = msg => _logger.LogInformation(msg),
                Compress = true,
                CreateBackup = true
            });

            currentStep++;
            MergeProgress = 100;

            // Show success message
            ShowSuccess($"Projects merged successfully!\nOutput saved to: {Path.GetFileName(OutputFilePath)}");

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

    private string NormalizePath(string path)
    {
        return Path.GetFullPath(path).Replace('/', Path.DirectorySeparatorChar);
    }
}