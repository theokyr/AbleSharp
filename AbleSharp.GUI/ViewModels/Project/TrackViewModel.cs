using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

public class TrackViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Track Track { get; }
    private string _trackName;
    private readonly int _colorIndex;

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

    // Color properties
    public string BaseColor => ColorPalette.GetColor(_colorIndex);
    public string LightColor => ColorPalette.GetLightColor(_colorIndex);
    public string DarkColor => ColorPalette.GetDarkColor(_colorIndex);

    public TrackViewModel(Track track)
    {
        Track = track;
        _trackName = track?.Name?.EffectiveName?.Val ?? "(Unnamed)";
        _colorIndex = track?.Color?.Val ?? 0;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}