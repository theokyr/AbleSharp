using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TrackViewModel> _tracks = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProjectViewModel(AbletonProject project)
        {
            // Convert the project's track info into TrackViewModel objects
            if (project?.LiveSet?.Tracks != null)
            {
                foreach (var track in project.LiveSet.Tracks)
                {
                    _tracks.Add(new TrackViewModel(track));
                }
            }
        }

        // Now it's a collection of *TrackViewModel* objects
        public ObservableCollection<TrackViewModel> Tracks
        {
            get => _tracks;
            set
            {
                if (_tracks != value)
                {
                    _tracks = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}