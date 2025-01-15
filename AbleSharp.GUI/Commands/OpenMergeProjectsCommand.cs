using System;
using System.Windows.Input;
using AbleSharp.GUI.Services;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.ViewModels.Tools;
using AbleSharp.GUI.Views.Tools;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace AbleSharp.GUI.Commands;

public class OpenMergeProjectsWindowCommand : ICommand
{
    private readonly ILogger<OpenMergeProjectsWindowCommand> _logger;

    public OpenMergeProjectsWindowCommand(MainWindowViewModel vm)
    {
        _logger = LoggerService.GetLogger<OpenMergeProjectsWindowCommand>();
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _logger.LogInformation("Opening Merge Projects Window");

        var mergeViewModel = App.Services.GetRequiredService<MergeProjectsViewModel>();

        var mergeWindow = new MergeProjectsWindow(mergeViewModel);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null } desktopLifetime)
        {
            mergeWindow.ShowDialog(desktopLifetime.MainWindow);
            return;
        }

        mergeWindow.Show();
    }

    public event EventHandler? CanExecuteChanged;
}