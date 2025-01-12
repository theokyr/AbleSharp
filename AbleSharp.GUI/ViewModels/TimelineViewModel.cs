using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using AbleSharp.Lib;
using ReactiveUI;
using System.Reactive.Concurrency;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Converters;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

namespace AbleSharp.GUI.ViewModels;

public class TimelineViewModel : INotifyPropertyChanged
{
    private decimal _tempo;
    private int _timeSigNumerator;
    private int _timeSigDenominator;
    private double _zoom = 80.0;
    private double _panX = 0.0;

    // Flattened list of all tracks for timeline display
    public ObservableCollection<TimelineTrackViewModel> Tracks { get; } = new();

    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }

    public TimelineViewModel(AbletonProject project)
    {
        // Read basic project info
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

        ZoomInCommand = AbleSharpUiCommand.Create(ZoomIn);
        ZoomOutCommand = AbleSharpUiCommand.Create(ZoomOut);
    }

    private void BuildTrackHierarchy(List<Track> tracks)
    {
        var trackDict = new Dictionary<string, TimelineTrackViewModel>();
        var processedTracks = new HashSet<string>();

        // First pass: Create all track ViewModels
        foreach (var track in tracks)
        {
            var vm = new TimelineTrackViewModel(track, this);
            trackDict[track.Id] = vm;
        }

        // Second pass: Build hierarchy and determine indentation
        void ProcessTrack(Track track, decimal indent = 0)
        {
            if (processedTracks.Contains(track.Id)) return;
            processedTracks.Add(track.Id);

            var vm = trackDict[track.Id];
            vm.IndentLevel = indent;

            // Add to our flattened track list
            Tracks.Add(vm);

            // Find and process child tracks
            var childTracks = tracks.Where(t =>
                t.TrackGroupId?.Val != null &&
                t.TrackGroupId.Val.ToString() == track.Id).ToList();

            foreach (var childTrack in childTracks) ProcessTrack(childTrack, indent + 1);
        }

        // Start with root tracks (those without a group or with invalid group)
        foreach (var track in tracks)
        {
            var groupId = track.TrackGroupId?.Val ?? -1;
            if (groupId == -1 || !trackDict.ContainsKey(groupId.ToString())) ProcessTrack(track);
        }

        // Process any remaining tracks
        foreach (var track in tracks)
            if (!processedTracks.Contains(track.Id))
                ProcessTrack(track);
    }

    #region Properties

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
            if (Math.Abs(_zoom - value) > 0.001)
            {
                _zoom = Math.Max(1.0, Math.Min(500.0, value));
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToPixelConverter));
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

    #endregion

    private void ZoomIn()
    {
        Zoom *= 1.2;
    }

    private void ZoomOut()
    {
        Zoom /= 1.2;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}