using AbleSharp.Lib;
using System.Text;
using AbleSharp.Lib; // For AbletonProject, LiveSet, etc.
using System.Collections.Generic;
using System.IO;

namespace AbleSharp.SDK;

public static class AbletonProjectDumper
{
    /// <summary>
    /// Outputs a human-readable, recursive debug dump of an entire AbletonProject object structure.
    /// </summary>
    public static string DebugDumpProject(AbletonProject project)
    {
        var sb = new StringBuilder();

        if (project == null)
        {
            sb.AppendLine("[AbletonProject is null]");
            return sb.ToString();
        }

        DumpProject(project, sb, 0);
        return sb.ToString();
    }

    #region Private Dump Methods

    private static void DumpProject(AbletonProject project, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, $"-- AbletonProject --");
        AppendLine(sb, indent + 1, $"MajorVersion: {project.MajorVersion}");
        AppendLine(sb, indent + 1, $"MinorVersion: {project.MinorVersion}");
        AppendLine(sb, indent + 1, $"SchemaChangeCount: {project.SchemaChangeCount}");
        AppendLine(sb, indent + 1, $"Creator: {project.Creator}");
        AppendLine(sb, indent + 1, $"Revision: {project.Revision}");

        // LiveSet
        if (project.LiveSet != null)
        {
            DumpLiveSet(project.LiveSet, sb, indent + 1);
        }
        else
        {
            AppendLine(sb, indent + 1, "[LiveSet is null]");
        }
    }

    private static void DumpLiveSet(LiveSet liveSet, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- LiveSet --");
        AppendLine(sb, indent + 1, $"NextPointeeId: {liveSet.NextPointeeId?.Val}");
        AppendLine(sb, indent + 1, $"OverwriteProtectionNumber: {liveSet.OverwriteProtectionNumber?.Val}");
        AppendLine(sb, indent + 1, $"LomId: {liveSet.LomId?.Val}");
        AppendLine(sb, indent + 1, $"LomIdView: {liveSet.LomIdView?.Val}");
        AppendLine(sb, indent + 1, $"AutomationMode: {liveSet.AutomationMode?.Val}");
        AppendLine(sb, indent + 1, $"SnapAutomationToGrid: {liveSet.SnapAutomationToGrid?.Val}");
        AppendLine(sb, indent + 1, $"ArrangementOverdub: {liveSet.ArrangementOverdub?.Val}");

        // Tracks
        if (liveSet.Tracks != null && liveSet.Tracks.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Tracks ({liveSet.Tracks.Count}):");
            for (int i = 0; i < liveSet.Tracks.Count; i++)
            {
                var track = liveSet.Tracks[i];
                DumpTrack(track, sb, indent + 2, i);
            }
        }
        else
        {
            AppendLine(sb, indent + 1, "[No Tracks]");
        }

        // Scenes
        if (liveSet.Scenes != null && liveSet.Scenes.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Scenes ({liveSet.Scenes.Count}):");
            for (int i = 0; i < liveSet.Scenes.Count; i++)
            {
                var scene = liveSet.Scenes[i];
                AppendLine(sb, indent + 2, $"[Scene {i}] Id={scene.Id}, Name={scene.Name?.Val}");
            }
        }

        // Transport
        if (liveSet.Transport != null)
        {
            DumpTransport(liveSet.Transport, sb, indent + 1);
        }

        // ... etc. (Dump MainTrack, PreHearTrack, SendsPre, Locators, Grid, ScaleInformation, TimeSelection, SequencerNavigator, etc.)
    }

    private static void DumpTrack(Track track, StringBuilder sb, int indent, int trackIndex)
    {
        // Attempt to detect track type:
        string trackTypeName = track.GetType().Name;
        AppendLine(sb, indent, $"-- Track[{trackIndex}]: {trackTypeName} --");
        AppendLine(sb, indent + 1, $"Id: {track.Id}");
        AppendLine(sb, indent + 1, $"LomId: {track.LomId?.Val}");
        AppendLine(sb, indent + 1, $"Name: {track.Name?.EffectiveName?.Val}");
        AppendLine(sb, indent + 1, $"Color: {track.Color?.Val}");
        AppendLine(sb, indent + 1, $"TrackUnfolded: {track.TrackUnfolded?.Val}");

        // If this is a "FreezableTrack", we can dump freeze info:
        if (track is FreezableTrack freezable)
        {
            AppendLine(sb, indent + 1, $"Freeze: {freezable.Freeze?.Val}");
            AppendLine(sb, indent + 1, $"NeedArrangerRefreeze: {freezable.NeedArrangerRefreeze?.Val}");
        }

        // GroupTrack => dump Slots
        if (track is GroupTrack groupTrack)
        {
            if (groupTrack.Slots != null && groupTrack.Slots.Count > 0)
            {
                AppendLine(sb, indent + 1, $"Slots ({groupTrack.Slots.Count}):");
                for (int i = 0; i < groupTrack.Slots.Count; i++)
                {
                    var slot = groupTrack.Slots[i];
                    AppendLine(sb, indent + 2, $"GroupTrackSlot[{i}]: Id={slot.Id}, LomId={slot.LomId?.Val}");
                }
            }
            else
            {
                AppendLine(sb, indent + 1, "[No Slots]");
            }
        }

        // If there is a DeviceChain in all tracks:
        if (track.DeviceChain != null)
        {
            DumpDeviceChain(track.DeviceChain, sb, indent + 1);
        }

        // ... etc. (Dump ClipSlotsListWrapper, TakeLanes, etc. if needed)
    }

    private static void DumpDeviceChain(DeviceChain deviceChain, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- DeviceChain --");
        // Example fields
        if (deviceChain.Devices != null)
        {
            AppendLine(sb, indent + 1, $"Devices Count: {deviceChain.Devices.Count}");
            for (int i = 0; i < deviceChain.Devices.Count; i++)
            {
                var dev = deviceChain.Devices[i];
                AppendLine(sb, indent + 2, $"Device[{i}] Id={dev.Id?.Val} IsExpanded={dev.IsExpanded?.Val}");
                // Dump device parameters, etc.
            }
        }
        else
        {
            AppendLine(sb, indent + 1, "[No Devices]");
        }

        // Dump mixers, input/output routing, freeze/main sequencers, etc., similarly
    }

    private static void DumpTransport(Transport transport, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- Transport --");
        AppendLine(sb, indent + 1, $"PhaseNudgeTempo: {transport.PhaseNudgeTempo?.Val}");
        AppendLine(sb, indent + 1, $"LoopOn: {transport.LoopOn?.Val}");
        AppendLine(sb, indent + 1, $"LoopStart: {transport.LoopStart?.Val}");
        // etc. for all Transport properties
    }

    /// <summary>
    /// Helper for indentation-based output.
    /// </summary>
    private static void AppendLine(StringBuilder sb, int indent, string text)
    {
        sb.AppendLine(new string(' ', indent * 2) + text);
    }

    #endregion
}