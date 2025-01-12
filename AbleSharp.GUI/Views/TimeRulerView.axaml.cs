using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbleSharp.GUI.Views;

public partial class TimeRulerView : UserControl
{
    private ItemsControl _tickMarks;
    private readonly List<TickMark> _ticks = new();

    public TimeRulerView()
    {
        InitializeComponent();
        _tickMarks = this.FindControl<ItemsControl>("TickMarks");
        _tickMarks.ItemsSource = _ticks;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Update the ruler based on zoom level and visible time range
    public void UpdateRuler(double pixelsPerBeat, double visibleStartBeat, double visibleEndBeat)
    {
        _ticks.Clear();

        // Calculate major tick spacing based on zoom level
        var majorTickSpacing = CalculateMajorTickSpacing(pixelsPerBeat);

        // Calculate visible range
        var startBeat = Math.Floor(visibleStartBeat / majorTickSpacing) * majorTickSpacing;
        var endBeat = Math.Ceiling(visibleEndBeat / majorTickSpacing) * majorTickSpacing;

        // Add ticks within the visible range
        for (var beat = startBeat; beat <= endBeat; beat += majorTickSpacing)
            _ticks.Add(new TickMark
            {
                Position = beat * pixelsPerBeat,
                Label = beat.ToString("0")
            });
    }

    private double CalculateMajorTickSpacing(double pixelsPerBeat)
    {
        // Adjust tick spacing based on zoom level
        // We want roughly 100 pixels between major ticks
        double pixelsBetweenTicks = 100;
        var beatsPerTick = pixelsBetweenTicks / pixelsPerBeat;

        // Round to nearest power of 2
        return Math.Pow(2, Math.Round(Math.Log2(beatsPerTick)));
    }
}

public class TickMark
{
    public double Position { get; set; }
    public string Label { get; set; }
}