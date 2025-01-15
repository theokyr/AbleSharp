using System;
using System.Windows.Input;
using Avalonia.Controls;
using AbleSharp.GUI.Services;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.ViewModels.Tools;
using AbleSharp.GUI.Views.Tools;

namespace AbleSharp.GUI.Commands;

public class OpenMergeProjectsWindowCommand : ICommand
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ILogger<OpenMergeProjectsWindowCommand> _logger;

    public OpenMergeProjectsWindowCommand(MainWindowViewModel vm)
    {
        _mainWindowViewModel = vm;
        _logger = LoggerService.GetLogger<OpenMergeProjectsWindowCommand>();
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _logger.LogInformation("Opening Merge Projects Window");

        var mergeWindow = new MergeProjectsWindow
        {
            DataContext = new MergeProjectsViewModel()
        };
        mergeWindow.Show();
    }

    public event EventHandler? CanExecuteChanged;
}