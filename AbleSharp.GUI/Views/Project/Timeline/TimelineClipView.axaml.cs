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

    public static readonly DirectProperty<TimelineClipView, double> ZoomProperty =
        AvaloniaProperty.RegisterDirect<TimelineClipView, double>(
            nameof(Zoom),
            o => o.Zoom,
            (o, v) => o.Zoom = v);

    private double _zoom = Constants.DEFAULT_ZOOM_LEVEL;

    public double Zoom
    {
        get => _zoom;
        set
        {
            if (SetAndRaise(ZoomProperty, ref _zoom, value)) UpdateClipPosition();
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
            Zoom = vm.Zoom;
            UpdateClipPosition();

            // Subscribe to zoom changes
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TimelineClipViewModel.Zoom)) Zoom = vm.Zoom;
            };
        }
    }

    private void UpdateClipPosition()
    {
        if (DataContext is TimelineClipViewModel vm)
        {
            var leftPos = (double)(vm.Time * (decimal)Zoom);
            var width = (double)(vm.Length * (decimal)Zoom);

            _logger.LogTrace($"Positioning clip '{vm.ClipName}' at x={leftPos}, width={width}");

            Canvas.SetLeft(_clipBorder, leftPos);
            _clipBorder.Width = width;
        }
    }
}