using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        private string _trackName;

        /// <summary>
        /// Children tracks (only if this is a GroupTrack).
        /// </summary>
        public ObservableCollection<TrackViewModel> Children { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public TrackViewModel(Track track)
        {
            Track = track;
            _trackName = track?.Name?.EffectiveName ?? "(Unnamed Track)";
        }

        public Track Track { get; }

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

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}