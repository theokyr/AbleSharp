using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using System.ComponentModel;
using Avalonia.Interactivity;

namespace AbleSharp.GUI.Views;

public partial class TrackHeaderView : UserControl
{
    private readonly ILogger<TrackHeaderView> _logger;
    private Border _meterForeground;
    private DispatcherTimer? _meterUpdateTimer;
    private Random _random = new(); // For demo meter movement

    public TrackHeaderView()
    {
        _logger = LoggerService.GetLogger<TrackHeaderView>();
        InitializeComponent();

        _meterForeground = this.FindControl<Border>("MeterForeground");

        // Start meter animation (for demo purposes)
        SetupMeterAnimation();

        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is TimelineTrackViewModel vm)
        {
            _logger.LogTrace($"Track header data context changed: {vm.TrackName}");

            // Reset meter animation when track changes
            ResetMeterAnimation();
        }
    }

    private void SetupMeterAnimation()
    {
        _meterUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };

        _meterUpdateTimer.Tick += (s, e) => UpdateMeter();
        _meterUpdateTimer.Start();

        _logger.LogTrace("Started meter animation timer");
    }

    private void ResetMeterAnimation()
    {
        _meterUpdateTimer?.Stop();
        _meterUpdateTimer?.Start();
        _meterForeground.Width = 0;
    }

    private void UpdateMeter()
    {
        // This is just for demo purposes - in reality we'd get this from audio level
        var meterBackground = this.FindControl<Border>("MeterBackground");
        var maxWidth = meterBackground.Bounds.Width;

        // Simulate some "bouncing" meter movement
        var currentWidth = _meterForeground.Width;
        var target = _random.NextDouble() * maxWidth * 0.8; // Max 80% of width

        // Smooth transition
        var newWidth = currentWidth + (target - currentWidth) * 0.2;
        _meterForeground.Width = newWidth;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _meterUpdateTimer?.Stop();
        _logger.LogTrace("Stopped meter animation timer");
    }
}