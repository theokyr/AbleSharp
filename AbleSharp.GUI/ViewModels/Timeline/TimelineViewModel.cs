using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
    private double _zoom = Constants.DEFAULT_ZOOM_LEVEL; // Pixel width per beat
    private double _panX = 0.0;
    private double _totalTimelineWidth;
    private decimal _lastClipEndTime = 0;

    // Zoom constants
    private const double MIN_ZOOM = 0.125;
    private const double MAX_ZOOM = 50.0;
    private const double ZOOM_STEP = 4;

    // Flattened list of all tracks for timeline display
    public ObservableCollection<TimelineTrackViewModel> Tracks { get; } = new();

    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }

    public TimelineViewModel(AbletonProject project)
    {
        _logger = LoggerService.GetLogger<TimelineViewModel>();
        _logger.LogDebug("Creating TimelineViewModel");

        // Basic project info (tempo, time sig, etc.)
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

        // Build track hierarchy
        if (project?.LiveSet?.Tracks != null) BuildTrackHierarchy(project.LiveSet.Tracks);

        // Zoom commands
        ZoomInCommand = AbleSharpUiCommand.Create(ZoomIn);
        ZoomOutCommand = AbleSharpUiCommand.Create(ZoomOut);

        UpdateTimelineWidth();

        _logger.LogDebug($"Timeline initialized with zoom={_zoom}, " +
                         $"tempo={_tempo}, timeSig={_timeSigNumerator}/{_timeSigDenominator}");
    }

    private void BuildTrackHierarchy(List<Track> tracks)
    {
        var trackDict = new Dictionary<string, TimelineTrackViewModel>();
        var processedTracks = new HashSet<string>();

        // 1) Create all track VMs and track the latest clip end time
        foreach (var track in tracks)
        {
            var vm = new TimelineTrackViewModel(track, this);
            trackDict[track.Id] = vm;

            // Track the latest clip end time across all clips
            foreach (var clipVM in vm.ClipViewModels) _lastClipEndTime = Math.Max(_lastClipEndTime, clipVM.Time + clipVM.Length);
        }

        // 2) Build hierarchy (indentation, children, etc.)
        void ProcessTrack(Track t, decimal indent = 0)
        {
            if (processedTracks.Contains(t.Id)) return;
            processedTracks.Add(t.Id);

            var tvm = trackDict[t.Id];
            tvm.IndentLevel = indent;

            Tracks.Add(tvm);

            // Recursively find child tracks
            var childTracks = tracks.Where(ct =>
                ct.TrackGroupId?.Val != null &&
                ct.TrackGroupId.Val.ToString() == t.Id
            ).ToList();

            foreach (var child in childTracks) ProcessTrack(child, indent + 1);
        }

        // Start w/ root tracks (TrackGroupId = -1 or invalid group)
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
        // Add some padding beyond the last clip
        var endPadding = 8m;
        var total = _lastClipEndTime + endPadding;
        var safeMinimum = 32m;

        var finalBeats = Math.Max(safeMinimum, total);

        // Convert beats to pixels with current zoom
        var newWidth = (double)(finalBeats * (decimal)Zoom);

        _logger.LogDebug($"Updating timeline width: LastClipEnd={_lastClipEndTime}, " +
                         $"Total={total}, FinalBeats={finalBeats}, " +
                         $"Zoom={Zoom}, NewWidth={newWidth}");

        TotalTimelineWidth = newWidth;
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

    public double Zoom
    {
        get => _zoom;
        set
        {
            var newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, value));
            if (Math.Abs(_zoom - newZoom) > 0.001)
            {
                _zoom = newZoom;
                OnPropertyChanged();
                UpdateTimelineWidth();

                _logger.LogDebug($"Zoom changed to {_zoom}");
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

    private void ZoomIn()
    {
        Zoom *= ZOOM_STEP;
        _logger.LogTrace("Zoomed in to {Zoom}", Zoom);
    }

    private void ZoomOut()
    {
        Zoom /= ZOOM_STEP;
        _logger.LogTrace("Zoomed out to {Zoom}", Zoom);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}