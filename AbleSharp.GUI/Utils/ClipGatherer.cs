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

        // MAIN
        var mainSeq = track.DeviceChain?.MainSequencer;
        if (mainSeq?.ClipTimeable?.ArrangerAutomation?.Events != null)
        {
            var clips = mainSeq.ClipTimeable.ArrangerAutomation.Events;
            _logger.LogTrace("Found {Count} clips in main-sequencer ClipTimeable.ArrangerAutomation", clips.Count);
            result.AddRange(clips);
        }

        if (mainSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            var eventsCount = mainSeq.Sample.ArrangerAutomation.Events.Count;
            _logger.LogTrace("Found {Count} Audio events in main-sequencer Sample.ArrangerAutomation", eventsCount);
            result.AddRange(mainSeq.Sample.ArrangerAutomation.Events);
        }

        // FREEZE
        var freezeSeq = track.DeviceChain?.FreezeSequencer;
        if (freezeSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            var freezeCount = freezeSeq.Sample.ArrangerAutomation.Events.Count;
            _logger.LogTrace("Found {Count} events in freeze-sequencer Sample.ArrangerAutomation", freezeCount);
            result.AddRange(freezeSeq.Sample.ArrangerAutomation.Events);
        }

        // Return what we found
        return result;
    }
}