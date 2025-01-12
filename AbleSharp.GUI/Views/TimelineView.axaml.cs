using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.Views;

public partial class TimelineView : UserControl
{
    private readonly ILogger<TimelineView> _logger;

    // The pinned time ruler at top
    private TimeRulerView _timeRuler;

    // The outer vertical scroller
    private ScrollViewer _outerVerticalScroller;

    // The inner horizontal scroller (for track lanes)
    private ScrollViewer _horizontalScroller;

    private TimelineViewModel _viewModel;

    public TimelineView()
    {
        _logger = LoggerService.GetLogger<TimelineView>();
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Grab references after XAML load
        _timeRuler = this.FindControl<TimeRulerView>("TimeRuler");
        _outerVerticalScroller = this.FindControl<ScrollViewer>("OuterVerticalScroller");
        _horizontalScroller = this.FindControl<ScrollViewer>("HorizontalScroller");

        // Whenever the user scrolls horizontally on the timeline lanes,
        // update the time ruler accordingly:
        _horizontalScroller.ScrollChanged += OnHorizontalScrollChanged;
    }

    private void OnDataContextChanged(object sender, EventArgs e)
    {
        if (DataContext is TimelineViewModel vm)
        {
            // Unsubscribe from old model if needed:
            if (_viewModel != null) _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;

            _viewModel = vm;
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

            // Force an initial ruler update
            UpdateTimeRuler();
        }
    }

    private void ViewModelOnPropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        // If the Zoom or timeline width changes, re‐draw the time ruler
        if (e.PropertyName == nameof(TimelineViewModel.Zoom) ||
            e.PropertyName == nameof(TimelineViewModel.TotalTimelineWidth))
            UpdateTimeRuler();
    }

    private void OnHorizontalScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        // Re‐draw the time ruler’s tick marks on horizontal scroll
        UpdateTimeRuler();
    }

    /// <summary>
    /// Grabs the current horizontal scroll offset (and viewport width)
    /// from the horizontal scroller, converts them to beats, and
    /// calls TimeRulerView.UpdateRuler().
    /// </summary>
    private void UpdateTimeRuler()
    {
        if (_viewModel == null)
            return;

        var pixelsPerBeat = _viewModel.Zoom;

        // The horizontal offset & width come from the horizontal scroller
        var scrollPosition = _horizontalScroller.Offset.X;
        var viewportWidth = _horizontalScroller.Viewport.Width;

        // Convert the offset & width to “beats”
        var startBeat = scrollPosition / pixelsPerBeat;
        var endBeat = (scrollPosition + viewportWidth) / pixelsPerBeat;

        _timeRuler.UpdateRuler(
            pixelsPerBeat,
            startBeat,
            endBeat,
            _viewModel.TotalTimelineWidth
        );
    }
}