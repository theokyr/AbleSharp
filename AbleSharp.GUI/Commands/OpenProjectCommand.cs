using System.Windows.Input;
using AbleSharp.GUI.Services;
using AbleSharp.GUI.ViewModels;
using AbleSharp.SDK;
using Microsoft.Extensions.Logging;

namespace AbleSharp.GUI.Commands;

public class OpenProjectCommand : ICommand
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ILogger<OpenProjectCommand> _logger;

    public OpenProjectCommand(MainWindowViewModel vm)
    {
        _mainWindowViewModel = vm;
        _logger = LoggerService.GetLogger<OpenProjectCommand>();
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        var filePath = parameter as string ?? await FileDialogService.ShowOpenFileDialogAsync();

        if (!string.IsNullOrEmpty(filePath))
        {
            _logger.LogInformation("Attempting to open Ableton project from '{FilePath}'", filePath);

            try
            {
                var project = AbletonProjectHandler.LoadFromFile(filePath);
                _logger.LogInformation("Successfully loaded project.");

                var projectViewModel = new ProjectViewModel(project);

                _mainWindowViewModel.ShowProjectView(projectViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project from '{FilePath}'", filePath);
            }
        }
        else
        {
            _logger.LogInformation("Open project dialog canceled or empty file path. '{parameter}'", parameter);
        }
    }

    public event EventHandler? CanExecuteChanged;
}