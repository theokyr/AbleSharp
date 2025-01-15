using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.Services;
using Microsoft.Extensions.DependencyInjection;
using AbleSharp.GUI.ViewModels;
using AbleSharp.GUI.ViewModels.Tools;

namespace AbleSharp.GUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        LoggerService.Initialize();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        var args = (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Args;
        var launchProject = args is { Length: > 0 } ? args[0] : null;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;

            if (!string.IsNullOrEmpty(launchProject))
            {
                var mainViewModel = mainWindow.DataContext as MainWindowViewModel;
                mainViewModel?.OpenProjectDialogCommand.Execute(launchProject);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MergeProjectsViewModel>();
        services.AddTransient<MainWindow>();
    }
}