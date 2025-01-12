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

    private decimal _time;
    private decimal _length;
    private string _clipName;

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

    public ClipViewModel(Clip clip)
    {
        _logger.LogDebug("Creating ClipViewModel for clip Id={Id}", clip.Id);

        // Store the arrangement position directly, this determines where the clip appears
        Time = clip.Time;

        // Calculate length from the clip's CurrentEnd - CurrentStart
        Length = (clip.CurrentEnd?.Val ?? 16) - (clip.CurrentStart?.Val ?? 0);

        ClipName = string.IsNullOrEmpty(clip.Name?.Val)
            ? "Unnamed Clip"
            : clip.Name.Val;

        _logger.LogDebug(
            "Created clip '{Name}': Time={Time}, Length={Length}",
            ClipName,
            Time,
            Length
        );
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public override string ToString()
    {
        return $"Clip '{ClipName}' Time={Time} Length={Length}";
    }
}