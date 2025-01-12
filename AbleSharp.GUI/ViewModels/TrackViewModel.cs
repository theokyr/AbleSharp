using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    /// <summary>
    /// Represents a track in a hierarchical view (TreeView).
    /// Each track can have children if it's a group track, etc.
    /// </summary>
    public class TrackViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Track Track { get; }
        private string _trackName;

        public string TrackName
        {
            get => _trackName;
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TrackViewModel> Children { get; } = new();

        public TrackViewModel(Track track)
        {
            Track = track;
            _trackName = track?.Name?.EffectiveName?.Val ?? "(Unnamed)";
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}