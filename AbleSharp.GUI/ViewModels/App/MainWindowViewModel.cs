using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Views;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using AbleSharp.GUI.Views.Tools;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AbleSharp.GUI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private object? _currentView;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand OpenProjectDialogCommand { get; }
    public ICommand OpenMergeProjectsWindowCommand { get; }
    public ICommand OpenDebugLogWindowCommand { get; }
    public ICommand OpenSettingsWindowCommand { get; }
    public ICommand OpenAboutWindowCommand { get; }
    public ICommand ExitCommand { get; }

    public MainWindowViewModel(string? loadProjectPath = null)
    {
        _logger = LoggerService.GetLogger<MainWindowViewModel>();

        _logger.LogDebug("Constructing MainWindowViewModel");

        OpenProjectDialogCommand = new OpenProjectDialogCommand(this);
        OpenMergeProjectsWindowCommand = new OpenMergeProjectsWindowCommand(this);

        OpenDebugLogWindowCommand = AbleSharpUiCommand.Create(ShowDebugLogWindow);

        OpenSettingsWindowCommand = AbleSharpUiCommand.Create(ShowSettingsWindow);

        OpenAboutWindowCommand = AbleSharpUiCommand.Create(ShowAboutWindow);

        ExitCommand = AbleSharpUiCommand.Create(Exit);

        if (loadProjectPath is null or "") return;

        OpenProjectDialogCommand.Execute(loadProjectPath);
    }

    public object CurrentView
    {
        get => _currentView;
        set
        {
            if (_currentView != value)
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void ShowProjectView(ProjectViewModel projectViewModel)
    {
        _logger.LogInformation("Switching current view to ProjectView");
        var projectView = new ProjectView
        {
            DataContext = projectViewModel
        };

        CurrentView = projectView;
    }

    private void ShowDebugLogWindow()
    {
        _logger.LogDebug("User requested to open debug log window");

        var debugLogWindow = new DebugLogWindow();
        debugLogWindow.Show();
    }

    private void ShowSettingsWindow()
    {
        _logger.LogDebug("User requested to open settings window");

        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();
    }

    private void ShowAboutWindow()
    {
        _logger.LogDebug("User requested to open about window");

        var aboutWindow = new AboutWindow();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null } desktopLifetime)
        {
            aboutWindow.ShowDialog(desktopLifetime.MainWindow);
            return;
        }

        aboutWindow.Show();
    }

    private void Exit()
    {
        _logger.LogDebug("Exiting Application");
        Environment.Exit(0);
    }
}