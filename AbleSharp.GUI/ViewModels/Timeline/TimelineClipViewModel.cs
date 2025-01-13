using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

public class TimelineClipViewModel : INotifyPropertyChanged
{
    private decimal _time;
    private decimal _length;
    private string _clipName;
    private double _zoomX;
    private double _zoomY;
    private readonly int _colorIndex;

    public event PropertyChangedEventHandler? PropertyChanged;

    public decimal Time
    {
        get => _time;
        private set
        {
            if (_time != value)
            {
                _time = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Length
    {
        get => _length;
        private set
        {
            if (_length != value)
            {
                _length = value;
                OnPropertyChanged();
            }
        }
    }

    public string ClipName
    {
        get => _clipName;
        private set
        {
            if (_clipName != value)
            {
                _clipName = value;
                OnPropertyChanged();
            }
        }
    }

    public double ZoomX
    {
        get => _zoomX;
        set
        {
            if (Math.Abs(_zoomX - value) > 0.001)
            {
                _zoomX = value;
                OnPropertyChanged();
            }
        }
    }

    public double ZoomY
    {
        get => _zoomY;
        set
        {
            if (Math.Abs(_zoomY - value) > 0.001)
            {
                _zoomY = value;
                OnPropertyChanged();
            }
        }
    }

    // Color properties
    public string BaseColor => ColorPalette.GetColor(_colorIndex);
    public string LightColor => ColorPalette.GetLightColor(_colorIndex);
    public string DarkColor => ColorPalette.GetDarkColor(_colorIndex);

    public TimelineClipViewModel(Clip clip, double zoomX, double zoomY)
    {
        Time = clip.Time;
        Length = (clip.CurrentEnd?.Val ?? 16) - (clip.CurrentStart?.Val ?? 0);
        ClipName = string.IsNullOrEmpty(clip.Name?.Val) ? "Unnamed Clip" : clip.Name.Val;
        ZoomX = zoomX;
        ZoomY = zoomY;
        _colorIndex = clip.Color?.Val ?? 0;
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}