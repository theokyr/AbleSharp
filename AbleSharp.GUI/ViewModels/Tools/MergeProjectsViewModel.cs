using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Services;
using Microsoft.Extensions.Logging;
using Avalonia.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using AbleSharp.Lib;
using AbleSharp.SDK;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Avalonia.ReactiveUI; // Ensure this is included

namespace AbleSharp.GUI.ViewModels.Tools;

public class MergeProjectsViewModel : ReactiveObject
{
    private readonly ILogger<MergeProjectsViewModel> _logger;
    private string _outputFilePath = string.Empty;
    private string _mergeSettingExample = "Default Setting";

    public ObservableCollection<string> SelectedProjects { get; set; } = new();

    public string OutputFilePath
    {
        get => _outputFilePath;
        set => this.RaiseAndSetIfChanged(ref _outputFilePath, value);
    }

    // Example Merge Settings
    public string MergeSettingExample
    {
        get => _mergeSettingExample;
        set => this.RaiseAndSetIfChanged(ref _mergeSettingExample, value);
    }

    public ReactiveCommand<Unit, Unit> AddProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedProjectsCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectOutputFileCommand { get; }
    public ReactiveCommand<Unit, Unit> MergeProjectsCommand { get; }

    public MergeProjectsViewModel()
    {
        _logger = LoggerService.GetLogger<MergeProjectsViewModel>();

        // Initialize Commands with AvaloniaScheduler.Instance to ensure UI thread execution
        AddProjectsCommand = ReactiveCommand.CreateFromTask(AddProjectsAsync, outputScheduler: AvaloniaScheduler.Instance);

        RemoveSelectedProjectsCommand = ReactiveCommand.Create(
            RemoveSelectedProjects,
            this.WhenAnyValue(vm => vm.SelectedProjects.Count, count => count > 0),
            AvaloniaScheduler.Instance
        );

        SelectOutputFileCommand = ReactiveCommand.CreateFromTask(SelectOutputFileAsync, outputScheduler: AvaloniaScheduler.Instance);

        MergeProjectsCommand = ReactiveCommand.CreateFromTask(
            MergeProjectsAsync,
            this.WhenAnyValue(
                vm => vm.SelectedProjects.Count,
                vm => vm.OutputFilePath,
                (count, outputFilePath) => count >= 2 && !string.IsNullOrEmpty(outputFilePath)
            ),
            AvaloniaScheduler.Instance
        );

        // ReactiveCommands automatically handle CanExecute; no need for manual RaiseCanExecuteChanged
    }

    /// <summary>
    /// Adds selected project files to the merge list.
    /// </summary>
    private async Task AddProjectsAsync()
    {
        _logger.LogInformation("Adding projects to merge");

        var filePaths = await FileDialogService.ShowOpenFilesDialogAsync();

        if (filePaths != null)
            foreach (var path in filePaths)
                if (!SelectedProjects.Contains(path))
                {
                    SelectedProjects.Add(path);
                    _logger.LogInformation($"Added project: {path}");
                }
    }

    /// <summary>
    /// Removes the last selected project from the merge list.
    /// </summary>
    private void RemoveSelectedProjects()
    {
        if (SelectedProjects.Count > 0)
        {
            var projectPath = SelectedProjects[SelectedProjects.Count - 1];
            SelectedProjects.Remove(projectPath);
            _logger.LogInformation($"Removed project: {projectPath}");
        }
    }

    /// <summary>
    /// Opens a save file dialog to select the output file for the merged project.
    /// </summary>
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

    /// <summary>
    /// Merges the selected Ableton Live projects and saves the result to the output file.
    /// </summary>
    private async Task MergeProjectsAsync()
    {
        try
        {
            _logger.LogInformation("Starting merge of projects.");

            if (SelectedProjects.Count < 2)
            {
                _logger.LogWarning("At least two projects are required to perform a merge.");
                return;
            }

            if (string.IsNullOrEmpty(OutputFilePath))
            {
                _logger.LogWarning("Output file path is not specified.");
                return;
            }

            // Load all projects asynchronously on background threads
            _logger.LogInformation("Loading projects...");
            var projects = new List<AbletonProject>();
            foreach (var path in SelectedProjects)
                try
                {
                    _logger.LogInformation($"Loading: {path}");
                    var project = await Task.Run(() => AbletonProjectHandler.LoadFromFile(path));
                    projects.Add(project);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load project: {path}");
                    // Optionally, notify the user about the failed project load
                }

            if (projects.Count < 2)
            {
                _logger.LogError("Not enough projects loaded to perform a merge.");
                return;
            }

            // Merge projects asynchronously on background threads
            _logger.LogInformation("Merging projects...");
            var mergedProject = await Task.Run(() => AbletonProjectMerger.MergeProjects(projects));

            // Save result asynchronously on background threads
            _logger.LogInformation($"Saving merged project to: {OutputFilePath}");
            await Task.Run(() => AbletonProjectHandler.SaveToFile(mergedProject, OutputFilePath));

            _logger.LogInformation("Merge completed successfully.");

            // Optionally, clear the selected projects or notify the user
            // Example: Clear selection after successful merge
            SelectedProjects.Clear();

            // Optionally, notify the user about successful merge
            // This can be implemented using a dialog service
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during project merge.");
            // Optionally, notify the user about the error
            // This can be implemented using a dialog service
        }
    }
}