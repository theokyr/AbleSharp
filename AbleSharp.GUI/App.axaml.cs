using AbleSharp.GUI.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        LoggerService.Initialize();

        var args = (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Args;
        var launchProject = args is { Length: > 0 } ? args[0] : null;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.MainWindow = new MainWindow(launchProject);

        base.OnFrameworkInitializationCompleted();
    }
}