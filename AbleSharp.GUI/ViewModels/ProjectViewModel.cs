// File: AbleSharp.GUI/ViewModels/ProjectViewModel.cs
using System.Collections.ObjectModel;
using System.Linq;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class ProjectViewModel
    {
        private ObservableCollection<TrackViewModel> _rootTracks = new();
        public ObservableCollection<TrackViewModel> RootTracks
        {
            get => _rootTracks;
            set { _rootTracks = value; }
        }

        public ObservableCollection<TimelineTrackViewModel> TimelineTracks { get; } = new();

        public ProjectViewModel(AbletonProject project)
        {
            if (project?.LiveSet?.Tracks != null)
            {
                BuildTrackHierarchy(project.LiveSet.Tracks);
                BuildTimelineTracks(project.LiveSet.Tracks);
            }
        }

        private void BuildTrackHierarchy(List<Track> tracks)
        {
            var allVms = new Dictionary<string, TrackViewModel>();
            foreach (var t in tracks)
            {
                var vm = new TrackViewModel(t);
                allVms[t.Id] = vm;
            }
            foreach (var t in tracks)
            {
                var tvm = allVms[t.Id];
                var parentIdVal = t.TrackGroupId?.Val ?? -1;
                var parentId = parentIdVal.ToString();
                if (parentIdVal != -1 && allVms.ContainsKey(parentId))
                {
                    allVms[parentId].Children.Add(tvm);
                }
                else
                {
                    RootTracks.Add(tvm);
                }
            }
        }

        private void BuildTimelineTracks(List<Track> tracks)
        {
            foreach (var t in tracks)
                TimelineTracks.Add(new TimelineTrackViewModel(t));
        }
    }
}