using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib; // so we can reference AbletonProject, Track, etc.

namespace AbleSharp.GUI.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _tracks = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProjectViewModel(AbletonProject project)
        {
            if (project?.LiveSet?.Tracks != null)
            {
                foreach (var track in project.LiveSet.Tracks)
                {
                    _tracks.Add(track?.Name?.EffectiveName ?? "(Unnamed Track)");
                }
            }
        }

        public ObservableCollection<string> Tracks
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