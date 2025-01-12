using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.Views;

public partial class GridLinesView : UserControl
{
    private readonly ILogger<GridLinesView> _logger;
    private readonly ObservableCollection<GridLineModel> _gridLines = new();
    private double _pixelsPerBeat = Constants.DEFAULT_ZOOM_LEVEL;
    private double _totalWidth;
    private ItemsControl _gridLinesControl;

    public GridLinesView()
    {
        _logger = LoggerService.GetLogger<GridLinesView>();
        InitializeComponent();

        _gridLinesControl = this.FindControl<ItemsControl>("GridLines");
        _gridLinesControl.ItemsSource = _gridLines;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void UpdateGrid(double pixelsPerBeat, double totalWidth)
    {
        _logger.LogDebug($"Updating grid - PPB: {pixelsPerBeat}, Width: {totalWidth}");

        _pixelsPerBeat = pixelsPerBeat;
        _totalWidth = totalWidth;
        Width = totalWidth;

        RegenerateGridLines();
    }

    private void RegenerateGridLines()
    {
        _gridLines.Clear();

        // Calculate total beats
        var totalBeats = _totalWidth / _pixelsPerBeat;

        _logger.LogTrace($"Generating grid lines for {totalBeats} beats");

        // Add vertical lines for each beat
        for (var beat = 0; beat <= totalBeats; beat++)
        {
            var x = beat * _pixelsPerBeat;

            // Determine line type
            var isBar = beat % 4 == 0;
            var isMeasure = beat % 16 == 0;

            var color = isMeasure ? "#3A3A3A" :
                isBar ? "#2A2A2A" :
                "#222222";

            var thickness = isMeasure ? 1.0 :
                isBar ? 0.75 :
                0.5;

            _gridLines.Add(new GridLineModel
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, 10000), // Make lines extend beyond view
                StrokeColor = color,
                StrokeThickness = thickness
            });
        }

        _logger.LogTrace($"Generated {_gridLines.Count} grid lines");
    }
}

public class GridLineModel
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public string StrokeColor { get; set; }
    public double StrokeThickness { get; set; }
}