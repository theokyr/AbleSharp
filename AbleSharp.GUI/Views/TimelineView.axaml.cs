using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using AbleSharp.GUI.ViewModels;

namespace AbleSharp.GUI.Views;

public partial class TimelineView : UserControl
{
    private TimeRulerView _timeRuler;
    private ScrollViewer _timelineScroller;
    private TimelineViewModel _viewModel;

    public TimelineView()
    {
        InitializeComponent();

        _timeRuler = this.FindControl<TimeRulerView>("TimeRuler");
        _timelineScroller = this.FindControl<ScrollViewer>("TimelineScroller");

        // Update ruler when scrolling or data context changes
        _timelineScroller.ScrollChanged += OnScrollChanged;
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
                if (e.PropertyName == nameof(TimelineViewModel.Zoom)) UpdateTimeRuler();
            };
            UpdateTimeRuler();
        }
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        UpdateTimeRuler();
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

        _timeRuler.UpdateRuler(pixelsPerBeat, startBeat, endBeat);
    }
}