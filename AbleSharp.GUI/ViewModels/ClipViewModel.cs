using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbleSharp.Lib;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.ViewModels;

public class ClipViewModel : INotifyPropertyChanged
{
    private readonly ILogger<ClipViewModel> _logger = LoggerService.GetLogger<ClipViewModel>();
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

    // For UI binding (Width in the timeline)
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
        _logger.LogDebug("Creating ClipViewModel for clip Id={Id}", clip.Id);

        // The arrangement time (where this clip appears in the timeline)
        Start = clip.Time;
        
        // Calculate end time based on clip length
        var clipLength = (clip.CurrentEnd?.Val ?? 16) - (clip.CurrentStart?.Val ?? 0);
        End = Start + clipLength;

        ClipName = string.IsNullOrEmpty(clip.Name?.Val)
            ? "Unnamed Clip"
            : clip.Name.Val;

        _logger.LogDebug(
            "Created clip '{Name}': Time={Time}, CurrentStart={CurStart}, CurrentEnd={CurEnd} => " +
            "DisplayedStart={DispStart}, DisplayedEnd={DispEnd}, Length={Length}",
            ClipName,
            clip.Time,
            clip.CurrentStart?.Val,
            clip.CurrentEnd?.Val,
            Start,
            End,
            Length
        );
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public override string ToString() => $"Clip '{ClipName}' Start={Start} End={End}";
}