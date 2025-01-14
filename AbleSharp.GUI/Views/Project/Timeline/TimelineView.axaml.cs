using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using System.ComponentModel;

namespace AbleSharp.GUI.Views;

public partial class TimelineView : UserControl
{
    private readonly ILogger<TimelineView> _logger;
    private TimelineViewModel? ViewModel;

    // UI Components
    private ScrollViewer _horizontalScroller;
    private ScrollViewer _headerScroller;
    private TimeRulerView _timeRuler;

    public TimelineView()
    {
        _logger = LoggerService.GetLogger<TimelineView>();
        InitializeComponent();

        _horizontalScroller = this.FindControl<ScrollViewer>("HorizontalScroller");
        _headerScroller = this.FindControl<ScrollViewer>("HeaderScroller");
        _timeRuler = this.FindControl<TimeRulerView>("TimeRuler");

        // Handle scroll synchronization
        _horizontalScroller.ScrollChanged += OnHorizontalScrollChanged;

        DataContextChanged += OnDataContextChanged;

        // Listen for size changes
        _horizontalScroller.PropertyChanged += OnScrollerPropertyChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnScrollerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == BoundsProperty || e.Property == ScrollViewer.ViewportProperty) UpdateViewportSize();
    }

    private void UpdateViewportSize()
    {
        if (ViewModel != null && _horizontalScroller.Viewport.Width > 0) ViewModel.SetViewportWidth(_horizontalScroller.Viewport.Width);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not TimelineViewModel vm) return;

        // Unsubscribe from old viewmodel if needed
        if (ViewModel != null) ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;

        ViewModel = vm;
        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;

        // Force initial updates
        UpdateTimeRuler();
        UpdateViewportSize();
    }

    private void OnHorizontalScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        // Sync vertical scrolling between timeline and headers
        if (e.OffsetDelta.Y != 0)
            _headerScroller.Offset = new Vector(
                _headerScroller.Offset.X,
                _horizontalScroller.Offset.Y
            );

        // Update ruler when scrolling horizontally
        if (e.OffsetDelta.X != 0) UpdateTimeRuler();
    }

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimelineViewModel.ZoomX) ||
            e.PropertyName == nameof(TimelineViewModel.TotalTimelineWidth))
        {
            _logger.LogDebug($"Timeline X zoom/width changed - ZoomX: {ViewModel?.ZoomX}, Width: {ViewModel?.TotalTimelineWidth}");
            UpdateTimeRuler();
        }
    }

    private void UpdateTimeRuler()
    {
        if (ViewModel == null) return;

        var scrollPosition = _horizontalScroller.Offset.X;
        var viewportWidth = _horizontalScroller.Viewport.Width;

        var startBeat = scrollPosition / ViewModel.ZoomX;
        var endBeat = (scrollPosition + viewportWidth) / ViewModel.ZoomX;

        _timeRuler.UpdateRuler(
            ViewModel.ZoomX,
            startBeat,
            endBeat,
            ViewModel.TotalTimelineWidth
        );
    }
}