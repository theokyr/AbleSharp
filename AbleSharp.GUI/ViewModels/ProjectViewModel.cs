using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TrackViewModel> _rootTracks = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProjectViewModel(AbletonProject project)
        {
            if (project?.LiveSet?.Tracks != null)
            {
                BuildTrackHierarchy(project.LiveSet.Tracks);
            }
        }

        /// <summary>
        /// The top-level tracks to display in a TreeView.
        /// </summary>
        public ObservableCollection<TrackViewModel> RootTracks
        {
            get => _rootTracks;
            set
            {
                if (_rootTracks != value)
                {
                    _rootTracks = value;
                    OnPropertyChanged();
                }
            }
        }

        private void BuildTrackHierarchy(List<Track> tracks)
        {
            var allViewModels = new Dictionary<string, TrackViewModel>();
            foreach (var track in tracks)
            {
                var vm = new TrackViewModel(track);
                if (!string.IsNullOrEmpty(track.Id))
                {
                    allViewModels[track.Id] = vm;
                }
            }

            foreach (var track in tracks)
            {
                var trackVm = allViewModels[track.Id];

                var groupIdVal = track.TrackGroupId?.Val ?? -1;
                string parentId = groupIdVal.ToString();

                // If groupIdVal == -1, it's root. 
                if (groupIdVal != -1 && allViewModels.ContainsKey(parentId))
                {
                    var parentVm = allViewModels[parentId];
                    parentVm.Children.Add(trackVm);
                }
                else
                {
                    RootTracks.Add(trackVm);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}