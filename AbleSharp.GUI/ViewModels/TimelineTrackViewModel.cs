// File: AbleSharp.GUI/ViewModels/TimelineTrackViewModel.cs
using System.Collections.ObjectModel;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class TimelineTrackViewModel
    {
        public Track Track { get; }
        public string TrackName => Track?.Name?.EffectiveName?.Val ?? "(No Name)";
        public ObservableCollection<ClipViewModel> Clips { get; } = new();

        public TimelineTrackViewModel(Track track)
        {
            Track = track;
            var found = ClipGatherer.FromTrack(track);
            foreach (var c in found)
                Clips.Add(new ClipViewModel(c));
        }
    }
}