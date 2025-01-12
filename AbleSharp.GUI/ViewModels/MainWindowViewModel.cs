using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Views;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AbleSharp.GUI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private object? _currentView;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand OpenProjectCommand { get; }
    public ICommand OpenDebugLogCommand { get; }
    public ICommand ExitCommand { get; }

    public MainWindowViewModel(string? loadProjectPath = null)
    {
        _logger = LoggerService.GetLogger<MainWindowViewModel>();

        _logger.LogDebug("Constructing MainWindowViewModel");

        OpenProjectCommand = new OpenProjectCommand(this);

        OpenDebugLogCommand = AbleSharpUiCommand.Create(ShowDebugLog);

        ExitCommand = AbleSharpUiCommand.Create(Exit);

        if (loadProjectPath is null or "")
        {
            return;
        }
        
        OpenProjectCommand.Execute(loadProjectPath);
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

    private void ShowDebugLog()
    {
        _logger.LogDebug("User requested to open debug log window");

        var debugLogWindow = new DebugLogWindow();
        debugLogWindow.Show();
    }

    private void Exit()
    {
        _logger.LogDebug("Exiting Application");
        Environment.Exit(0);
    }
}