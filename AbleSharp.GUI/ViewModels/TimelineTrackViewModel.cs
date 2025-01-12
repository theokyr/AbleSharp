using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

/// <summary>
/// Each track row in the timeline. Contains multiple ClipViewModels.
/// Supports track hierarchy with Children collection.
/// </summary>
public class TimelineTrackViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private TimelineViewModel? _parentTimeline;
    private decimal _indentLevel;

    public Track Track { get; }
    public string TrackName => Track?.Name?.EffectiveName?.Val ?? "(No Name)";
    public ObservableCollection<ClipViewModel> Clips { get; } = new();
    public ObservableCollection<TimelineTrackViewModel> Children { get; } = new();

    public decimal IndentLevel
    {
        get => _indentLevel;
        set
        {
            if (_indentLevel != value)
            {
                _indentLevel = value;
                OnPropertyChanged();
            }
        }
    }

    public TimelineTrackViewModel(Track track, TimelineViewModel parent, decimal indent = 0)
    {
        Track = track;
        _parentTimeline = parent;
        _indentLevel = indent;

        var found = ClipGatherer.FromTrack(track);
        foreach (var c in found)
        {
            var clipVm = new ClipViewModel(c);
            Clips.Add(clipVm);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public override string ToString()
    {
        return $"Track '{TrackName}' with {Clips.Count} clips";
    }
}