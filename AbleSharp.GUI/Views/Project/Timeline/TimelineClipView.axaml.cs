using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.Views;

public partial class TimelineClipView : UserControl
{
    private readonly ILogger<TimelineClipView> _logger;
    private Canvas _rootCanvas;
    private Border _clipBorder;

    public static readonly DirectProperty<TimelineClipView, double> ZoomXProperty =
        AvaloniaProperty.RegisterDirect<TimelineClipView, double>(
            nameof(ZoomX),
            o => o.ZoomX,
            (o, v) => o.ZoomX = v);

    public static readonly DirectProperty<TimelineClipView, double> ZoomYProperty =
        AvaloniaProperty.RegisterDirect<TimelineClipView, double>(
            nameof(ZoomY),
            o => o.ZoomY,
            (o, v) => o.ZoomY = v);

    private double _zoomX = Constants.DEFAULT_ZOOM_LEVEL_X;
    private double _zoomY = Constants.DEFAULT_ZOOM_LEVEL_Y;

    public double ZoomX
    {
        get => _zoomX;
        set
        {
            if (SetAndRaise(ZoomXProperty, ref _zoomX, value)) UpdateClipPosition();
        }
    }

    public double ZoomY
    {
        get => _zoomY;
        set
        {
            if (SetAndRaise(ZoomYProperty, ref _zoomY, value)) UpdateClipPosition();
        }
    }

    public TimelineClipView()
    {
        _logger = LoggerService.GetLogger<TimelineClipView>();
        InitializeComponent();

        _rootCanvas = this.FindControl<Canvas>("RootCanvas");
        _clipBorder = this.FindControl<Border>("ClipBorder");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is TimelineClipViewModel vm)
        {
            ZoomX = vm.ZoomX;
            ZoomY = vm.ZoomY;
            UpdateClipPosition();

            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TimelineClipViewModel.ZoomX)) ZoomX = vm.ZoomX;
                if (args.PropertyName == nameof(TimelineClipViewModel.ZoomY)) ZoomY = vm.ZoomY;
            };
        }
    }

    private void UpdateClipPosition()
    {
        if (DataContext is TimelineClipViewModel vm)
        {
            var leftPos = (double)(vm.Time * (decimal)ZoomX);
            var width = (double)(vm.Length * (decimal)ZoomX);

            _logger.LogTrace($"Positioning clip '{vm.ClipName}' at x={leftPos}, width={width}");

            Canvas.SetLeft(_clipBorder, leftPos);
            _clipBorder.Width = width;
        }
    }
}