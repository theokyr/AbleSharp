﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using System.ComponentModel;

namespace AbleSharp.GUI.Views;

public partial class TimelineTrackHeaderView : UserControl
{
    private readonly ILogger<TimelineTrackHeaderView> _logger;

    public TimelineTrackHeaderView()
    {
        _logger = LoggerService.GetLogger<TimelineTrackHeaderView>();
        InitializeComponent();

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

            // Make sure the header updates when zoom changes
            vm.PropertyChanged += TrackViewModel_PropertyChanged;
        }
    }

    private void TrackViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimelineTrackViewModel.ZoomY)) InvalidateVisual();
    }
}