using AbleSharp.Lib;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using Avalonia;

namespace AbleSharp.GUI;

public static class ClipGatherer
{
    private static readonly ILogger _logger = LoggerService.GetLogger<Application>();

    public static IEnumerable<Clip> FromTrack(Track track)
    {
        var result = new List<Clip>();

        _logger.LogDebug("Gathering clips from track '{TrackName}'", track.Name?.EffectiveName?.Val);

        var mainSeq = track.DeviceChain?.MainSequencer;
        if (mainSeq?.ClipSlotList != null)
            foreach (var slot in mainSeq.ClipSlotList)
                if (slot?.ClipData?.Clip != null)
                {
                    _logger.LogTrace("Found main-sequencer clip '{ClipName}'", slot.ClipData.Clip.Name?.Val);
                    result.Add(slot.ClipData.Clip);
                }

        // Audio Clips
        if (mainSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            var count = mainSeq.Sample.ArrangerAutomation.Events.Count();
            _logger.LogTrace("Found {Count} Audio events in main-sequencer Sample.ArrangerAutomation", count);
            result.AddRange(mainSeq.Sample.ArrangerAutomation.Events);
        }
        
        // Midi Clips
        if (mainSeq?.ClipTimeable?.ArrangerAutomation?.Events != null)
        {
            var clips = mainSeq.ClipTimeable.ArrangerAutomation.Events;
            _logger.LogTrace("Found {Count} MIDI clips in main-sequencer ClipTimeable.ArrangerAutomation", clips.Count);
            result.AddRange(clips);
        }

        // Freeze Samples
        var freezeSeq = track.DeviceChain?.FreezeSequencer;
        if (freezeSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            var count = freezeSeq.Sample.ArrangerAutomation.Events.Count();
            _logger.LogTrace("Found {Count} events in freeze-sequencer Sample.ArrangerAutomation", count);
            result.AddRange(freezeSeq.Sample.ArrangerAutomation.Events);
        }

        return result;
    }
}