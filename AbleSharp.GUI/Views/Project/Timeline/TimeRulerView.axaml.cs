using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.Views;

public partial class TimeRulerView : UserControl
{
    private readonly ILogger<TimeRulerView> _logger;
    private ItemsControl _tickMarks;
    private readonly ObservableCollection<TickMarkModel> _ticks = new();
    private double _pixelsPerBeat = Constants.DEFAULT_ZOOM_LEVEL;
    private double _viewportStart = 0.0;
    private double _viewportEnd = 16.0;
    private double _totalWidth;

    public TimeRulerView()
    {
        _logger = LoggerService.GetLogger<TimeRulerView>();
        InitializeComponent();
        _tickMarks = this.FindControl<ItemsControl>("TickMarks");
        _tickMarks.ItemsSource = _ticks;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void UpdateRuler(double pixelsPerBeat, double viewportStart, double viewportEnd, double totalWidth)
    {
        _logger.LogTrace("Updating ruler: PPB={PixelsPerBeat}, Start={Start}, End={End}, Width={Width}",
            pixelsPerBeat, viewportStart, viewportEnd, totalWidth);

        _pixelsPerBeat = pixelsPerBeat;
        _viewportStart = Math.Floor(viewportStart);
        _viewportEnd = Math.Ceiling(viewportEnd);
        _totalWidth = totalWidth;

        Width = totalWidth;
        UpdateTickMarks();
    }

    private void UpdateTickMarks()
    {
        _ticks.Clear();

        // Calculate spacing between major ticks based on zoom level
        var majorTickSpacing = CalculateMajorTickSpacing(_pixelsPerBeat);
        var minorTickCount = CalculateMinorTickCount(majorTickSpacing);

        // Generate ticks within the viewport
        var startBeat = Math.Floor(_viewportStart / majorTickSpacing) * majorTickSpacing;
        var endBeat = Math.Ceiling(_viewportEnd / majorTickSpacing) * majorTickSpacing;

        for (var beat = startBeat; beat <= endBeat; beat += majorTickSpacing)
        {
            var minorTicks = new ObservableCollection<MinorTickModel>();

            // Add minor tick marks between major ticks
            var minorSpacing = majorTickSpacing / (double)minorTickCount;
            for (var i = 1; i < minorTickCount; i++)
            {
                var minorBeat = beat + i * minorSpacing;
                if (minorBeat <= _viewportEnd)
                    minorTicks.Add(new MinorTickModel
                    {
                        Position = (minorBeat - beat) * _pixelsPerBeat,
                        StartPoint = new Point(0, 22),
                        EndPoint = new Point(0, 30)
                    });
            }

            _ticks.Add(new TickMarkModel
            {
                Position = beat * _pixelsPerBeat,
                Label = beat.ToString("0"),
                MinorTicks = minorTicks
            });
        }

        _logger.LogTrace("Generated {Count} major ticks", _ticks.Count);
    }

    private int CalculateMajorTickSpacing(double pixelsPerBeat)
    {
        // Adjust major tick spacing based on zoom level
        // Target about 100 pixels between major ticks
        var targetPixels = 100.0;
        var beatsPerMajorTick = targetPixels / pixelsPerBeat;

        // Round to nearest appropriate division
        if (beatsPerMajorTick <= 0.25) return 1; // Quarter beat
        if (beatsPerMajorTick <= 0.5) return 1; // Half beat
        if (beatsPerMajorTick <= 1) return 1; // One beat
        if (beatsPerMajorTick <= 2) return 2; // Two beats
        if (beatsPerMajorTick <= 4) return 4; // Bar (in 4/4)
        if (beatsPerMajorTick <= 8) return 8; // Two bars
        if (beatsPerMajorTick <= 16) return 16; // Four bars
        return 32; // Eight bars
    }

    private int CalculateMinorTickCount(int majorSpacing)
    {
        // More appropriate subdivisions based on major spacing
        return majorSpacing switch
        {
            1 => 4, // Quarter beats
            2 => 2, // Half beats
            4 => 4, // Beats
            8 => 8, // Two beats
            16 => 4, // Four beats
            32 => 8, // Eight beats
            _ => 4
        };
    }
}

public class TickMarkModel
{
    public double Position { get; set; }
    public string Label { get; set; }
    public ObservableCollection<MinorTickModel> MinorTicks { get; set; }
}

public class MinorTickModel
{
    public double Position { get; set; }
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
}