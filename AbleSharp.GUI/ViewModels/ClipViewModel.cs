using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

/// <summary>
/// Represents a single clip. 
/// </summary>
public class ClipViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private decimal _start;
    private decimal _end;
    private string _clipName;

    public decimal Start
    {
        get => _start;
        set
        {
            if (_start != value)
            {
                _start = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal End
    {
        get => _end;
        set
        {
            if (_end != value)
            {
                _end = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// A handy read-only property for (End - Start).
    /// We'll bind this to the Width in XAML.
    /// </summary>
    public decimal Length => End - Start;

    public string ClipName
    {
        get => _clipName;
        set
        {
            if (_clipName != value)
            {
                _clipName = value;
                OnPropertyChanged();
            }
        }
    }

    public ClipViewModel(Clip clip)
    {
        Start = clip.CurrentStart?.Val ?? 0;
        End = clip.CurrentEnd?.Val ?? 0;
        ClipName = clip.Name?.Val ?? "Unnamed Clip";
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public override string ToString()
    {
        return $"Clip '{ClipName}' from {Start} to {End} (Len={Length})";
    }
}