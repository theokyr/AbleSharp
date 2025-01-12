﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

public class TimelineClipViewModel : INotifyPropertyChanged
{
    private decimal _time;
    private decimal _length;
    private string _clipName;
    private double _zoom;
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

    public double Zoom
    {
        get => _zoom;
        set
        {
            if (Math.Abs(_zoom - value) > 0.001)
            {
                _zoom = value;
                OnPropertyChanged();
            }
        }
    }

    // Color properties
    public string BaseColor => ColorPalette.GetColor(_colorIndex);
    public string LightColor => ColorPalette.GetLightColor(_colorIndex);
    public string DarkColor => ColorPalette.GetDarkColor(_colorIndex);

    public TimelineClipViewModel(Clip clip, double zoom)
    {
        Time = clip.Time;
        Length = (clip.CurrentEnd?.Val ?? 16) - (clip.CurrentStart?.Val ?? 0);
        ClipName = string.IsNullOrEmpty(clip.Name?.Val) ? "Unnamed Clip" : clip.Name.Val;
        Zoom = zoom;

        // Parse color index, defaulting to 0 if not valid
        _colorIndex = clip.Color?.Val ?? 0;
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}