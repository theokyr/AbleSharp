using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels;

/// <summary>
/// A separate ViewModel to manage the global timeline:
/// - zoom in/out
/// - pan offset
/// - track/clip arrangement
/// - optional measure lines or time-based lines
/// </summary>
public class TimelineViewModel : INotifyPropertyChanged
{
    // The tempo and time signature could come from the Transport or MasterTrack
    private decimal _tempo;
    private int _timeSigNumerator;
    private int _timeSigDenominator;

    // Zoom factor in pixels-per-beat (or measure). Adjust as needed.
    private double _zoom = 80.0;

    // Pan or horizontal offset in pixels (for future advanced panning).
    private double _panX = 0.0;

    // The timeline row viewmodels:
    public ObservableCollection<TimelineTrackViewModel> Tracks { get; } = new();

    // Simple commands for UI zoom in/out, etc.
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }

    public TimelineViewModel(AbletonProject project)
    {
        // Example: read the global tempo from LiveSet.Transport or MasterTrack.
        // This is an example; adapt to your actual data logic:
        _tempo = project?.LiveSet?.MainTrack?.Tempo?.Val ?? 120m;

        // If time signatures exist, read them. Otherwise fallback:
        _timeSigNumerator = 4;
        _timeSigDenominator = 4;
        var mainTS = project?.LiveSet?.MainTrack?.TimeSignature?.TimeSignatures;
        if (mainTS != null && mainTS.Count > 0)
        {
            var first = mainTS[0];
            _timeSigNumerator = first.Numerator?.Val ?? 4;
            _timeSigDenominator = first.Denominator?.Val ?? 4;
        }

        // Build timeline tracks
        var allTracks = project?.LiveSet?.Tracks;
        if (allTracks != null)
        {
            foreach (var t in allTracks)
            {
                Tracks.Add(new TimelineTrackViewModel(t));
            }
        }

        // Set up commands
        ZoomInCommand = new RelayCommand(_ => ZoomIn(), _ => true);
        ZoomOutCommand = new RelayCommand(_ => ZoomOut(), _ => true);
    }

    public decimal Tempo
    {
        get => _tempo;
        set
        {
            if (_tempo != value)
            {
                _tempo = value;
                OnPropertyChanged();
            }
        }
    }

    public int TimeSigNumerator
    {
        get => _timeSigNumerator;
        set
        {
            if (_timeSigNumerator != value)
            {
                _timeSigNumerator = value;
                OnPropertyChanged();
            }
        }
    }

    public int TimeSigDenominator
    {
        get => _timeSigDenominator;
        set
        {
            if (_timeSigDenominator != value)
            {
                _timeSigDenominator = value;
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

    public double PanX
    {
        get => _panX;
        set
        {
            if (Math.Abs(_panX - value) > 0.001)
            {
                _panX = value;
                OnPropertyChanged();
            }
        }
    }

    private void ZoomIn()
    {
        // Example: increase zoom by 20%
        Zoom *= 1.2;
    }

    private void ZoomOut()
    {
        // Example: decrease zoom by ~16%
        Zoom /= 1.2;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

/// <summary>
/// Simple command helper for inline commands
/// (instead of using MVVM frameworks).
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?> _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute(parameter);
    public void Execute(object? parameter) => _execute(parameter);
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}