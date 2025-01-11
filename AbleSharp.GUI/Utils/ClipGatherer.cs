using AbleSharp.Lib;

namespace AbleSharp.GUI;

public static class ClipGatherer
{
    public static IEnumerable<Clip> FromTrack(Track track)
    {
        var result = new List<Clip>();

        // 1) ClipSlots in MainSequencer
        var mainSeq = track.DeviceChain?.MainSequencer;
        if (mainSeq?.ClipSlotList != null)
        {
            foreach (var slot in mainSeq.ClipSlotList)
            {
                if (slot?.ClipData?.Clip != null)
                    result.Add(slot.ClipData.Clip);
            }
        }

        // 2) ArrangerAutomation in MainSequencer.Sample
        //    The example <AudioClip> appears in: <Sample><ArrangerAutomation><Events>...
        if (mainSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            result.AddRange(mainSeq.Sample.ArrangerAutomation.Events);
        }

        // 3) Possibly check FreezeSequencer in track.DeviceChain.FreezeSequencer, too:
        var freezeSeq = track.DeviceChain?.FreezeSequencer;
        if (freezeSeq?.Sample?.ArrangerAutomation?.Events != null)
        {
            result.AddRange(freezeSeq.Sample.ArrangerAutomation.Events);
        }

        // If you have more potential clip locations, add them here.

        return result;
    }
}