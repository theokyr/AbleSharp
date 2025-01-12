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
    private double _pixelsPerBeat = 80.0;
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
        int majorTickSpacing = CalculateMajorTickSpacing(_pixelsPerBeat);
        int minorTickCount = CalculateMinorTickCount(majorTickSpacing);

        // Generate ticks within the viewport
        double startBeat = Math.Floor(_viewportStart / majorTickSpacing) * majorTickSpacing;
        double endBeat = Math.Ceiling(_viewportEnd / majorTickSpacing) * majorTickSpacing;

        for (double beat = startBeat; beat <= endBeat; beat += majorTickSpacing)
        {
            var minorTicks = new ObservableCollection<MinorTickModel>();
            
            // Add minor tick marks between major ticks
            double minorSpacing = majorTickSpacing / (double)minorTickCount;
            for (int i = 1; i < minorTickCount; i++)
            {
                double minorBeat = beat + (i * minorSpacing);
                if (minorBeat <= _viewportEnd)
                {
                    minorTicks.Add(new MinorTickModel
                    {
                        Position = (minorBeat - beat) * _pixelsPerBeat,
                        StartPoint = new Point(0, 22),
                        EndPoint = new Point(0, 30)
                    });
                }
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
        // We want roughly 100 pixels between major ticks
        double targetPixels = 100.0;
        double beatsPerMajorTick = targetPixels / pixelsPerBeat;

        // Round to nearest power of 2 or multiple of 4
        if (beatsPerMajorTick <= 1) return 1;
        if (beatsPerMajorTick <= 2) return 2;
        if (beatsPerMajorTick <= 4) return 4;
        if (beatsPerMajorTick <= 8) return 8;
        return 16;
    }

    private int CalculateMinorTickCount(int majorSpacing)
    {
        // Number of minor ticks between major ticks
        switch (majorSpacing)
        {
            case 1: return 4;  // Quarter beat divisions
            case 2: return 8;  // Eighth beat divisions
            case 4: return 4;  // Beat divisions
            case 8: return 8;  // Two beat divisions
            default: return 4; // Four beat divisions
        }
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