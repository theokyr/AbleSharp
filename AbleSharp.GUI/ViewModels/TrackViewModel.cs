// File: AbleSharp.GUI/ViewModels/TrackViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _trackName;
        public Track Track { get; }
        public string TrackName
        {
            get => _trackName;
            set { if (_trackName != value) { _trackName = value; OnPropertyChanged(); } }
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