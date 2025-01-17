using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using ReactiveUI;
using Avalonia.ReactiveUI;
using System.Reactive;
using Avalonia.Controls;

namespace AbleSharp.GUI.ViewModels.Tools;

public class AboutViewModel : ReactiveObject
{
    private readonly ILogger<AboutViewModel> _logger;
    private string _versionString;
    private string _osVersion;
    private string _runtimeVersion;
    private string _memoryUsage;
    private string _processId;

    public ReactiveCommand<Unit, Unit> OpenGitHubCommand { get; }
    public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }

    public AboutViewModel()
    {
        _logger = LoggerService.GetLogger<AboutViewModel>();

        // Initialize system information
        InitializeSystemInfo();
        StartMemoryMonitoring();

        // Initialize commands
        OpenGitHubCommand = ReactiveCommand.Create(
            OpenGitHub,
            outputScheduler: AvaloniaScheduler.Instance
        );
    }

    private void InitializeSystemInfo()
    {
        try
        {
            // Get version info
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString = $"Version {version?.Major}.{version?.Minor}.{version?.Build}";

            // Get OS info
            OsVersion = RuntimeInformation.OSDescription;

            // Get runtime info
            RuntimeVersion = RuntimeInformation.FrameworkDescription;

            // Get process info
            ProcessId = Process.GetCurrentProcess().Id.ToString();

            _logger.LogDebug("System information initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize system information");

            // Set fallback values
            VersionString = "Version Unknown";
            OsVersion = "Unknown OS";
            RuntimeVersion = "Unknown Runtime";
            ProcessId = "Unknown";
        }
    }

    private void StartMemoryMonitoring()
    {
        // Update memory usage initially
        UpdateMemoryUsage();

        // Start a timer to update memory usage periodically
        var timer = new System.Timers.Timer(5000); // 5 seconds
        timer.Elapsed += (s, e) => { UpdateMemoryUsage(); };
        timer.Start();
    }

    private void UpdateMemoryUsage()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var memoryMB = process.WorkingSet64 / 1024 / 1024;
            MemoryUsage = $"{memoryMB:N0} MB";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update memory usage");
            MemoryUsage = "Unknown";
        }
    }

    private void OpenGitHub()
    {
        try
        {
            var url = "https://github.com/yourusername/ablesharp";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Process.Start("open", url);

            _logger.LogInformation("Opened GitHub repository");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open GitHub repository");
        }
    }

    public string VersionString
    {
        get => _versionString;
        private set => this.RaiseAndSetIfChanged(ref _versionString, value);
    }

    public string OsVersion
    {
        get => _osVersion;
        private set => this.RaiseAndSetIfChanged(ref _osVersion, value);
    }

    public string RuntimeVersion
    {
        get => _runtimeVersion;
        private set => this.RaiseAndSetIfChanged(ref _runtimeVersion, value);
    }

    public string MemoryUsage
    {
        get => _memoryUsage;
        private set => this.RaiseAndSetIfChanged(ref _memoryUsage, value);
    }

    public string ProcessId
    {
        get => _processId;
        private set => this.RaiseAndSetIfChanged(ref _processId, value);
    }
}