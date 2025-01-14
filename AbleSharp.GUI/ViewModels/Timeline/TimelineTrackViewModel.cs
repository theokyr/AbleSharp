using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

public class TimelineTrackViewModel : INotifyPropertyChanged
{
    private decimal _indentLevel;
    public double ZoomX => ParentTimeline.ZoomX;
    public double ZoomY => ParentTimeline.ZoomY;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TimelineViewModel ParentTimeline { get; }
    public Track Track { get; }
    public string TrackName => Track?.Name?.EffectiveName?.Val ?? "(No Name)";
    public ObservableCollection<TimelineClipViewModel> ClipViewModels { get; } = new();
    public ObservableCollection<TimelineTrackViewModel> Children { get; } = new();

    // Color properties
    public string BaseColor => ColorPalette.GetColor(Track?.Color.Val ?? 0);
    public string LightColor => ColorPalette.GetLightColor(Track?.Color.Val ?? 0);
    public string DarkColor => ColorPalette.GetDarkColor(Track?.Color.Val ?? 0);

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
        ParentTimeline = parent;
        _indentLevel = indent;

        // Subscribe to parent's zoom changes
        ParentTimeline.PropertyChanged += ParentTimelinePropertyChanged;

        var found = ClipGatherer.FromTrack(track);
        foreach (var clip in found)
        {
            var clipVm = new TimelineClipViewModel(clip, ZoomX, ZoomY);
            ClipViewModels.Add(clipVm);
        }
    }

    private void ParentTimelinePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimelineViewModel.ZoomX))
        {
            foreach (var clipVm in ClipViewModels)
                clipVm.ZoomX = ZoomX;
            OnPropertyChanged(nameof(ZoomX));
        }
        else if (e.PropertyName == nameof(TimelineViewModel.ZoomY))
        {
            foreach (var clipVm in ClipViewModels)
                clipVm.ZoomY = ZoomY;
            OnPropertyChanged(nameof(ZoomY));
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}