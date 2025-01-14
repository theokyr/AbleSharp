using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using ReactiveUI;
using Avalonia.ReactiveUI;
using System.Reactive;

namespace AbleSharp.GUI.ViewModels.Tools;

public class SettingsViewModel : ReactiveObject
{
    private readonly ILogger<SettingsViewModel> _logger;

    // Theme settings
    private string _selectedTheme = "System";
    private string _defaultProjectLocation = "";

    // Timeline settings
    private bool _enableSnapping = true;
    private string _selectedSnapDivision = "1/8";
    private double _defaultZoomX = Constants.DEFAULT_ZOOM_LEVEL_X;
    private double _defaultZoomY = Constants.DEFAULT_ZOOM_LEVEL_Y;

    // Advanced settings
    private string _selectedLogLevel = "Info";
    private bool _enableDetailedLogging = false;
    private bool _enableHardwareAcceleration = true;

    // Commands
    public ReactiveCommand<Unit, Unit> BrowseProjectLocationCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetToDefaultCommand { get; }

    public SettingsViewModel()
    {
        _logger = LoggerService.GetLogger<SettingsViewModel>();

        // Initialize commands
        BrowseProjectLocationCommand = ReactiveCommand.CreateFromTask(
            async () => await BrowseForProjectLocation(),
            outputScheduler: AvaloniaScheduler.Instance
        );

        SaveSettingsCommand = ReactiveCommand.Create(
            SaveSettings,
            outputScheduler: AvaloniaScheduler.Instance
        );

        ResetToDefaultCommand = ReactiveCommand.Create(
            ResetToDefaults,
            outputScheduler: AvaloniaScheduler.Instance
        );

        LoadSettings();
    }

    private async Task BrowseForProjectLocation()
    {
        var result = await FileDialogService.ShowOpenFileDialogAsync();
        if (!string.IsNullOrEmpty(result))
        {
            DefaultProjectLocation = result;
            _logger.LogInformation($"Updated default project location: {result}");
        }
    }

    private void SaveSettings()
    {
        _logger.LogInformation("Saving settings...");

        try
        {
            // Here we would persist settings to disk
            // For now, just log the values
            _logger.LogDebug($"Theme: {SelectedTheme}");
            _logger.LogDebug($"Project Location: {DefaultProjectLocation}");
            _logger.LogDebug($"Snapping: {EnableSnapping}");
            _logger.LogDebug($"Snap Division: {SelectedSnapDivision}");
            _logger.LogDebug($"Default Zoom X: {DefaultZoomX}");
            _logger.LogDebug($"Default Zoom Y: {DefaultZoomY}");
            _logger.LogDebug($"Log Level: {SelectedLogLevel}");
            _logger.LogDebug($"Detailed Logging: {EnableDetailedLogging}");
            _logger.LogDebug($"Hardware Acceleration: {EnableHardwareAcceleration}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings");
        }
    }

    private void LoadSettings()
    {
        _logger.LogInformation("Loading settings...");

        try
        {
            // Here we would load settings from disk
            // For now, just set defaults
            ResetToDefaults();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings");
        }
    }

    private void ResetToDefaults()
    {
        _logger.LogInformation("Resetting settings to defaults...");

        SelectedTheme = "System";
        DefaultProjectLocation = "";
        EnableSnapping = true;
        SelectedSnapDivision = "1/8";
        DefaultZoomX = Constants.DEFAULT_ZOOM_LEVEL_X;
        DefaultZoomY = Constants.DEFAULT_ZOOM_LEVEL_Y;
        SelectedLogLevel = "Info";
        EnableDetailedLogging = false;
        EnableHardwareAcceleration = true;
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }

    public string DefaultProjectLocation
    {
        get => _defaultProjectLocation;
        set => this.RaiseAndSetIfChanged(ref _defaultProjectLocation, value);
    }

    public bool EnableSnapping
    {
        get => _enableSnapping;
        set => this.RaiseAndSetIfChanged(ref _enableSnapping, value);
    }

    public string SelectedSnapDivision
    {
        get => _selectedSnapDivision;
        set => this.RaiseAndSetIfChanged(ref _selectedSnapDivision, value);
    }

    public double DefaultZoomX
    {
        get => _defaultZoomX;
        set => this.RaiseAndSetIfChanged(ref _defaultZoomX, value);
    }

    public double DefaultZoomY
    {
        get => _defaultZoomY;
        set => this.RaiseAndSetIfChanged(ref _defaultZoomY, value);
    }

    public string SelectedLogLevel
    {
        get => _selectedLogLevel;
        set => this.RaiseAndSetIfChanged(ref _selectedLogLevel, value);
    }

    public bool EnableDetailedLogging
    {
        get => _enableDetailedLogging;
        set => this.RaiseAndSetIfChanged(ref _enableDetailedLogging, value);
    }

    public bool EnableHardwareAcceleration
    {
        get => _enableHardwareAcceleration;
        set => this.RaiseAndSetIfChanged(ref _enableHardwareAcceleration, value);
    }
}