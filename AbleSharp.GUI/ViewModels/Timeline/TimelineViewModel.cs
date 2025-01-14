using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using AbleSharp.Lib;
using ReactiveUI;
using AbleSharp.GUI.Commands;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.ViewModels;

public class TimelineViewModel : INotifyPropertyChanged
{
    private readonly ILogger<TimelineViewModel> _logger;
    private decimal _tempo;
    private int _timeSigNumerator;
    private int _timeSigDenominator;
    private double _zoomX = Constants.DEFAULT_ZOOM_LEVEL_X;
    private double _zoomY = Constants.DEFAULT_ZOOM_LEVEL_Y;
    private double _panX = 0.0;
    private double _totalTimelineWidth;
    private decimal _lastClipEndTime = 0;
    private double _viewportWidth = 800; // Default viewport width
    private double _viewportHeight = 600; // Default viewport height

    public ObservableCollection<TimelineTrackViewModel> Tracks { get; } = new();

    public ICommand ZoomInXCommand { get; }
    public ICommand ZoomOutXCommand { get; }
    public ICommand ZoomInYCommand { get; }
    public ICommand ZoomOutYCommand { get; }
    public ICommand FitHorizontalCommand { get; }
    public ICommand FitVerticalCommand { get; }

    public TimelineViewModel(AbletonProject project)
    {
        _logger = LoggerService.GetLogger<TimelineViewModel>();
        _logger.LogDebug("Creating TimelineViewModel");

        _tempo = project?.LiveSet?.MainTrack?.Tempo?.Val ?? 120m;
        _timeSigNumerator = 4;
        _timeSigDenominator = 4;

        var mainTS = project?.LiveSet?.MainTrack?.TimeSignature?.TimeSignatures;
        if (mainTS != null && mainTS.Count > 0)
        {
            var first = mainTS[0];
            _timeSigNumerator = first.Numerator?.Val ?? 4;
            _timeSigDenominator = first.Denominator?.Val ?? 4;
        }

        if (project?.LiveSet?.Tracks != null) BuildTrackHierarchy(project.LiveSet.Tracks);

        ZoomInXCommand = AbleSharpUiCommand.Create(ZoomInX);
        ZoomOutXCommand = AbleSharpUiCommand.Create(ZoomOutX);
        ZoomInYCommand = AbleSharpUiCommand.Create(ZoomInY);
        ZoomOutYCommand = AbleSharpUiCommand.Create(ZoomOutY);
        FitHorizontalCommand = AbleSharpUiCommand.Create(FitHorizontal);
        FitVerticalCommand = AbleSharpUiCommand.Create(FitVertical);

        UpdateTimelineWidth();

        _logger.LogDebug($"Timeline initialized with zoomX={_zoomX}, zoomY={_zoomY}, " +
                         $"tempo={_tempo}, timeSig={_timeSigNumerator}/{_timeSigDenominator}");
    }

    private void BuildTrackHierarchy(List<Track> tracks)
    {
        var trackDict = new Dictionary<string, TimelineTrackViewModel>();
        var processedTracks = new HashSet<string>();

        foreach (var track in tracks)
        {
            var vm = new TimelineTrackViewModel(track, this);
            trackDict[track.Id] = vm;

            foreach (var clipVM in vm.ClipViewModels)
                _lastClipEndTime = Math.Max(_lastClipEndTime, clipVM.Time + clipVM.Length);
        }

        void ProcessTrack(Track t, decimal indent = 0)
        {
            if (processedTracks.Contains(t.Id)) return;
            processedTracks.Add(t.Id);

            var tvm = trackDict[t.Id];
            tvm.IndentLevel = indent;

            Tracks.Add(tvm);

            var childTracks = tracks.Where(ct =>
                ct.TrackGroupId?.Val != null &&
                ct.TrackGroupId.Val.ToString() == t.Id
            ).ToList();

            foreach (var child in childTracks) ProcessTrack(child, indent + 1);
        }

        foreach (var t in tracks)
        {
            var groupId = t.TrackGroupId?.Val ?? -1;
            if (groupId == -1 || !trackDict.ContainsKey(groupId.ToString())) ProcessTrack(t);
        }

        _logger.LogDebug("Built track hierarchy with {Count} total tracks in timeline", Tracks.Count);
        UpdateTimelineWidth();
    }

    private void UpdateTimelineWidth()
    {
        var endPadding = 8m;
        var total = _lastClipEndTime + endPadding;
        var safeMinimum = 32m;
        var finalBeats = Math.Max(safeMinimum, total);
        var newWidth = (double)(finalBeats * (decimal)ZoomX);

        _logger.LogDebug($"Updating timeline width: LastClipEnd={_lastClipEndTime}, " +
                         $"Total={total}, FinalBeats={finalBeats}, " +
                         $"ZoomX={ZoomX}, NewWidth={newWidth}");

        TotalTimelineWidth = newWidth;
    }

    public void SetViewportWidth(double width)
    {
        if (Math.Abs(_viewportWidth - width) > 0.001)
        {
            _viewportWidth = width;
            _logger.LogDebug($"Viewport width updated to {width}");
        }
    }

    public void SetViewportHeight(double height)
    {
        if (Math.Abs(_viewportHeight - height) > 0.001)
        {
            _viewportHeight = height;
            _logger.LogDebug($"Viewport height updated to {height}");
        }
    }

    private void FitHorizontal()
    {
        if (_viewportWidth <= 0 || _lastClipEndTime <= 0)
        {
            _logger.LogWarning("Cannot fit horizontal - invalid viewport width or timeline length");
            return;
        }

        // Calculate the zoom level needed to fit the content
        var padding = 20; // pixels of padding
        var availableWidth = _viewportWidth - padding;
        var totalBeats = (double)_lastClipEndTime;
        var newZoom = availableWidth / totalBeats;

        // Clamp to min/max zoom levels
        newZoom = Math.Max(Constants.MIN_ZOOM_X, Math.Min(Constants.MAX_ZOOM_X, newZoom));

        _logger.LogDebug($"Fitting horizontal: viewport={_viewportWidth}, " +
                         $"beats={totalBeats}, newZoom={newZoom}");

        ZoomX = newZoom;
    }

    private void FitVertical()
    {
        if (_viewportHeight <= 0 || Tracks.Count == 0)
        {
            _logger.LogWarning("Cannot fit vertical - invalid viewport height or no tracks");
            return;
        }

        // Calculate the zoom level needed to fit the content
        var padding = 20; // pixels of padding
        var availableHeight = _viewportHeight - padding;
        var baseTrackHeight = 60.0; // Our base track height
        var totalTracks = Tracks.Count;
        var newZoom = availableHeight / (baseTrackHeight * totalTracks);

        // Clamp to min/max zoom levels
        newZoom = Math.Max(Constants.MIN_ZOOM_Y, Math.Min(Constants.MAX_ZOOM_Y, newZoom));

        _logger.LogDebug($"Fitting vertical: viewport={_viewportHeight}, " +
                         $"tracks={totalTracks}, newZoom={newZoom}");

        ZoomY = newZoom;
    }

    public decimal Tempo
    {
        get => _tempo;
        set
        {
            if (_tempo != value)
            {
                _tempo = value;
                OnPropertyChanged();
            }
        }
    }

    public int TimeSigNumerator
    {
        get => _timeSigNumerator;
        set
        {
            if (_timeSigNumerator != value)
            {
                _timeSigNumerator = value;
                OnPropertyChanged();
            }
        }
    }

    public int TimeSigDenominator
    {
        get => _timeSigDenominator;
        set
        {
            if (_timeSigDenominator != value)
            {
                _timeSigDenominator = value;
                OnPropertyChanged();
            }
        }
    }

    public double ZoomX
    {
        get => _zoomX;
        set
        {
            var newZoom = Math.Max(Constants.MIN_ZOOM_X, Math.Min(Constants.MAX_ZOOM_X, value));
            if (Math.Abs(_zoomX - newZoom) > 0.001)
            {
                _zoomX = newZoom;
                OnPropertyChanged();
                UpdateTimelineWidth();
                _logger.LogDebug($"ZoomX changed to {_zoomX}");
            }
        }
    }

    public double ZoomY
    {
        get => _zoomY;
        set
        {
            var newZoom = Math.Max(Constants.MIN_ZOOM_Y, Math.Min(Constants.MAX_ZOOM_Y, value));
            if (Math.Abs(_zoomY - newZoom) > 0.001)
            {
                _zoomY = newZoom;
                OnPropertyChanged();
                _logger.LogDebug($"ZoomY changed to {_zoomY}");
            }
        }
    }

    public double PanX
    {
        get => _panX;
        set
        {
            if (Math.Abs(_panX - value) > 0.001)
            {
                _panX = value;
                OnPropertyChanged();
            }
        }
    }

    public double TotalTimelineWidth
    {
        get => _totalTimelineWidth;
        private set
        {
            if (Math.Abs(_totalTimelineWidth - value) > 0.001)
            {
                _totalTimelineWidth = value;
                OnPropertyChanged();
            }
        }
    }

    private void ZoomInX()
    {
        ZoomX *= Constants.ZOOM_STEP;
        _logger.LogTrace("Zoomed in X to {ZoomX}", ZoomX);
    }

    private void ZoomOutX()
    {
        ZoomX /= Constants.ZOOM_STEP;
        _logger.LogTrace("Zoomed out X to {ZoomX}", ZoomX);
    }

    private void ZoomInY()
    {
        ZoomY *= Constants.ZOOM_STEP;
        _logger.LogTrace("Zoomed in Y to {ZoomY}", ZoomY);
    }

    private void ZoomOutY()
    {
        ZoomY /= Constants.ZOOM_STEP;
        _logger.LogTrace("Zoomed out Y to {ZoomY}", ZoomY);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}