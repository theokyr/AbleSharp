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
    private TimeRulerView _timeRuler;
    private ScrollViewer _timelineScroller;
    private ScrollViewer _rulerScroller;
    private TimelineViewModel _viewModel;

    public TimelineView()
    {
        _logger = LoggerService.GetLogger<TimelineView>();
        InitializeComponent();

        _timeRuler = this.FindControl<TimeRulerView>("TimeRuler");
        _timelineScroller = this.FindControl<ScrollViewer>("TimelineScroller");
        _rulerScroller = this.FindControl<ScrollViewer>("RulerScroller");

        // Sync ruler with main timeline scrolling
        _timelineScroller.ScrollChanged += OnMainScrollChanged;
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnDataContextChanged(object sender, EventArgs e)
    {
        if (DataContext is TimelineViewModel vm)
        {
            _viewModel = vm;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TimelineViewModel.Zoom) ||
                    e.PropertyName == nameof(TimelineViewModel.TotalTimelineWidth))
                {
                    UpdateTimeRuler();
                }
            };
            UpdateTimeRuler();
        }
    }

    private void OnMainScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentDelta != null && e.ExtentDelta.X != 0)
        {
            _logger.LogTrace("Timeline extent changed to {Extent}", e.ExtentDelta.X);
        }

        if (Math.Abs(e.OffsetDelta.X) > 0.001)
        {
            _logger.LogTrace("Timeline scrolled to {Offset}", e.OffsetDelta.X);
            // Keep the ruler in sync with main timeline
            _rulerScroller.Offset = new Vector(e.OffsetDelta.X, 0);
            UpdateTimeRuler();
        }
    }

    private void UpdateTimeRuler()
    {
        if (_viewModel == null) return;

        var pixelsPerBeat = _viewModel.Zoom;
        var scrollPosition = _timelineScroller.Offset.X;
        var viewportWidth = _timelineScroller.Viewport.Width;

        // Convert scroll position to beats
        var startBeat = scrollPosition / pixelsPerBeat;
        var endBeat = (scrollPosition + viewportWidth) / pixelsPerBeat;

        _timeRuler.UpdateRuler(pixelsPerBeat, startBeat, endBeat, _viewModel.TotalTimelineWidth);
    }
}