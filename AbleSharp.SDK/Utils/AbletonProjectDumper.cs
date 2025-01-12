using AbleSharp.Lib;
using System.Text;

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
        AppendLine(sb, indent + 1, $"GlobalQuantisation: {liveSet.GlobalQuantisation?.Val}");
        AppendLine(sb, indent + 1, $"AutoQuantisation: {liveSet.AutoQuantisation?.Val}");
        AppendLine(sb, indent + 1, $"MidiFoldIn: {liveSet.MidiFoldIn?.Val}");
        AppendLine(sb, indent + 1, $"MidiFoldMode: {liveSet.MidiFoldMode?.Val}");
        AppendLine(sb, indent + 1, $"MultiClipFocusMode: {liveSet.MultiClipFocusMode?.Val}");
        AppendLine(sb, indent + 1, $"MultiClipLoopBarHeight: {liveSet.MultiClipLoopBarHeight?.Val}");
        AppendLine(sb, indent + 1, $"MidiPrelisten: {liveSet.MidiPrelisten?.Val}");
        AppendLine(sb, indent + 1, $"InKey: {liveSet.InKey?.Val}");
        AppendLine(sb, indent + 1, $"SmpteFormat: {liveSet.SmpteFormat?.Val}");
        AppendLine(sb, indent + 1, $"IsContentSplitterOpen: {liveSet.IsContentSplitterOpen?.Val}");
        AppendLine(sb, indent + 1, $"IsExpressionSplitterOpen: {liveSet.IsExpressionSplitterOpen?.Val}");
        AppendLine(sb, indent + 1, $"ChooserBar: {liveSet.ChooserBar?.Val}");
        AppendLine(sb, indent + 1, $"Annotation: {liveSet.Annotation?.Val}");
        AppendLine(sb, indent + 1, $"SoloOrPflSavedValue: {liveSet.SoloOrPflSavedValue?.Val}");
        AppendLine(sb, indent + 1, $"SoloInPlace: {liveSet.SoloInPlace?.Val}");
        AppendLine(sb, indent + 1, $"CrossfadeCurve: {liveSet.CrossfadeCurve?.Val}");
        AppendLine(sb, indent + 1, $"LatencyCompensation: {liveSet.LatencyCompensation?.Val}");
        AppendLine(sb, indent + 1, $"HighlightedTrackIndex: {liveSet.HighlightedTrackIndex?.Val}");
        AppendLine(sb, indent + 1, $"ColorSequenceIndex: {liveSet.ColorSequenceIndex?.Val}");
        AppendLine(sb, indent + 1, $"AccidentalSpellingPreference: {liveSet.AccidentalSpellingPreference?.Val}");
        AppendLine(sb, indent + 1, $"PreferFlatRootNote: {liveSet.PreferFlatRootNote?.Val}");
        AppendLine(sb, indent + 1, $"UseWarperLegacyHiQMode: {liveSet.UseWarperLegacyHiQMode?.Val}");

        // Dump Grid
        if (liveSet.Grid != null)
            DumpGrid(liveSet.Grid, sb, indent + 1);

        // Dump ScaleInformation
        if (liveSet.ScaleInformation != null)
            DumpScaleInformation(liveSet.ScaleInformation, sb, indent + 1);

        // Dump TimeSelection
        if (liveSet.TimeSelection != null)
            DumpTimeSelection(liveSet.TimeSelection, sb, indent + 1);

        // Dump SequencerNavigator
        if (liveSet.SequencerNavigator != null)
            DumpSequencerNavigator(liveSet.SequencerNavigator, sb, indent + 1);

        // Dump ViewData
        if (liveSet.ViewData != null)
            AppendLine(sb, indent + 1, $"ViewData: {liveSet.ViewData.Value}");

        // Dump ViewStates
        if (liveSet.ViewStates != null)
            DumpViewStates(liveSet.ViewStates, sb, indent + 1);

        // Dump ExpressionLanes
        if (liveSet.ExpressionLanes != null && liveSet.ExpressionLanes.Count > 0)
        {
            AppendLine(sb, indent + 1, $"ExpressionLanes ({liveSet.ExpressionLanes.Count}):");
            for (int i = 0; i < liveSet.ExpressionLanes.Count; i++)
            {
                var el = liveSet.ExpressionLanes[i];
                AppendLine(sb, indent + 2, $"ExpressionLane[{i}]: Id={el.Id}, Type={el.Type?.Val}, Size={el.Size?.Val}, IsMinimized={el.IsMinimized?.Val}");
            }
        }

        // Dump ContentLanes
        if (liveSet.ContentLanes != null && liveSet.ContentLanes.Count > 0)
        {
            AppendLine(sb, indent + 1, $"ContentLanes ({liveSet.ContentLanes.Count}):");
            for (int i = 0; i < liveSet.ContentLanes.Count; i++)
            {
                var cl = liveSet.ContentLanes[i];
                AppendLine(sb, indent + 2, $"ContentLane[{i}]: Type={cl.Type?.Val}, Size={cl.Size?.Val}, IsMinimized={cl.IsMinimized?.Val}");
            }
        }

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
                DumpScene(scene, sb, indent + 2, i);
            }
        }

        // Transport
        if (liveSet.Transport != null)
        {
            DumpTransport(liveSet.Transport, sb, indent + 1);
        }

        // Locators
        if (liveSet.Locators != null && liveSet.Locators.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Locators ({liveSet.Locators.Count}):");
            foreach (var locator in liveSet.Locators)
            {
                DumpLocator(locator, sb, indent + 2);
            }
        }

        // SendsPre
        if (liveSet.SendsPre != null && liveSet.SendsPre.Count > 0)
        {
            AppendLine(sb, indent + 1, $"SendsPre ({liveSet.SendsPre.Count}):");
            for (int i = 0; i < liveSet.SendsPre.Count; i++)
            {
                var s = liveSet.SendsPre[i];
                AppendLine(sb, indent + 2, $"[SendsPre {i}] = {s.Value?.Val}");
            }
        }

        // MainTrack / PreHearTrack
        if (liveSet.MainTrack != null)
        {
            AppendLine(sb, indent + 1, "[MasterTrack:]");
            DumpTrack(liveSet.MainTrack, sb, indent + 2, -1);
        }
        if (liveSet.PreHearTrack != null)
        {
            AppendLine(sb, indent + 1, "[PreHearTrack:]");
            DumpTrack(liveSet.PreHearTrack, sb, indent + 2, -1);
        }

        // GroovePool
        if (liveSet.GroovePool != null)
        {
            DumpGroovePool(liveSet.GroovePool, sb, indent + 1);
        }
    }

    private static void DumpTrack(Track track, StringBuilder sb, int indent, int trackIndex)
    {
        // Attempt to detect track type:
        string indexPrefix = (trackIndex >= 0) ? $"[{trackIndex}]" : "";
        string trackTypeName = track.GetType().Name;
        AppendLine(sb, indent, $"-- Track{indexPrefix}: {trackTypeName} --");
        AppendLine(sb, indent + 1, $"Id: {track.Id}");
        AppendLine(sb, indent + 1, $"LomId: {track.LomId?.Val}");
        AppendLine(sb, indent + 1, $"Name: {track.Name?.EffectiveName?.Val}");
        AppendLine(sb, indent + 1, $"Color: {track.Color?.Val}");
        AppendLine(sb, indent + 1, $"TrackUnfolded: {track.TrackUnfolded?.Val}");
        AppendLine(sb, indent + 1, $"PreferredContentViewMode: {track.PreferredContentViewMode?.Val}");
        AppendLine(sb, indent + 1, $"LinkedTrackGroupId: {track.LinkedTrackGroupId?.Val}");
        AppendLine(sb, indent + 1, $"ViewData: {track.ViewData?.Val}");
        // etc. for other base Track properties if needed

        if (track.TrackDelay?.Value != null)
        {
            AppendLine(sb, indent + 1, $"TrackDelay Value: {track.TrackDelay.Value.Val}");
            AppendLine(sb, indent + 1, $"TrackDelay IsValueSampleBased: {track.TrackDelay.IsValueSampleBased?.Val}");
        }

        // If this is a "FreezableTrack", we can dump freeze info:
        if (track is FreezableTrack freezable)
        {
            AppendLine(sb, indent + 1, $"Freeze: {freezable.Freeze?.Val}");
            AppendLine(sb, indent + 1, $"NeedArrangerRefreeze: {freezable.NeedArrangerRefreeze?.Val}");
            AppendLine(sb, indent + 1, $"PostProcessFreezeClips: {freezable.PostProcessFreezeClips?.Val}");
        }

        // AudioTrack / MidiTrack / GroupTrack specifics:
        if (track is MidiTrack midiTrack)
        {
            AppendLine(sb, indent + 1, $"PitchbendRange: {midiTrack.PitchbendRange?.Val}");
            AppendLine(sb, indent + 1, $"IsTuned: {midiTrack.IsTuned?.Val}");
            AppendLine(sb, indent + 1, $"SavedPlayingSlot: {midiTrack.SavedPlayingSlot?.Val}");
            AppendLine(sb, indent + 1, $"SavedPlayingOffset: {midiTrack.SavedPlayingOffset?.Val}");
            AppendLine(sb, indent + 1, $"VelocityDetail: {midiTrack.VelocityDetail?.Val}");
            if (midiTrack.ControllerLayoutCustomization != null)
            {
                DumpControllerLayoutCustomization(midiTrack.ControllerLayoutCustomization, sb, indent + 2);
            }
        }
        if (track is AudioTrack audioTrack)
        {
            AppendLine(sb, indent + 1, $"SavedPlayingSlot: {audioTrack.SavedPlayingSlot?.Val}");
            AppendLine(sb, indent + 1, $"SavedPlayingOffset: {audioTrack.SavedPlayingOffset?.Val}");
            AppendLine(sb, indent + 1, $"VelocityDetail: {audioTrack.VelocityDetail?.Val}");
        }
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

        // Dump TakeLanes if needed
        if (track.TakeLanes != null)
        {
            AppendLine(sb, indent + 1, "-- TakeLanes --");
            if (track.TakeLanes.LaneCollection != null)
            {
                for (int i = 0; i < track.TakeLanes.LaneCollection.Count; i++)
                {
                    var lane = track.TakeLanes.LaneCollection[i];
                    AppendLine(sb, indent + 2, $"TakeLane[{i}] Id={lane.Id?.Val}");
                    AppendLine(sb, indent + 3, $"Height={lane.Height?.Val}");
                    AppendLine(sb, indent + 3, $"IsContentSelectedInDocument={lane.IsContentSelectedInDocument?.Val}");
                    AppendLine(sb, indent + 3, $"Name={lane.Name?.Val}");
                    AppendLine(sb, indent + 3, $"Annotation={lane.Annotation?.Val}");
                    if (lane.ClipAutomation != null && lane.ClipAutomation.Events != null)
                    {
                        AppendLine(sb, indent + 3, $"ClipAutomation MidiClip count: {lane.ClipAutomation.Events.Count}");
                        // Possibly loop over lane.ClipAutomation.Events
                    }
                }
            }
            AppendLine(sb, indent + 1, $"AreTakeLanesFolded={track.TakeLanes.AreTakeLanesFolded?.Val}");
        }

        // ClipSlotsListWrapper if you want
        if (track.ClipSlotsListWrapper != null)
        {
            AppendLine(sb, indent + 1, $"ClipSlotsListWrapper LomId: {track.ClipSlotsListWrapper.LomId}");
        }

        // AutomationEnvelopes
        if (track.AutomationEnvelopes != null && track.AutomationEnvelopes.Envelopes != null)
        {
            AppendLine(sb, indent + 1, $"AutomationEnvelopes Count: {track.AutomationEnvelopes.Envelopes.Count}");
            foreach (var envelope in track.AutomationEnvelopes.Envelopes)
            {
                AppendLine(sb, indent + 2, $"Envelope Id={envelope.Id}");
                if (envelope.Automation != null && envelope.Automation.Events != null)
                    AppendLine(sb, indent + 3, $"AutomationEvents Count={envelope.Automation.Events.Count}");
            }
        }
    }

    private static void DumpDeviceChain(DeviceChain deviceChain, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- DeviceChain --");

        // AutomationLanes
        if (deviceChain.AutomationLanes?.Lanes != null)
        {
            AppendLine(sb, indent + 1, $"AutomationLanes ({deviceChain.AutomationLanes.Lanes.Count}):");
            for (int i = 0; i < deviceChain.AutomationLanes.Lanes.Count; i++)
            {
                var lane = deviceChain.AutomationLanes.Lanes[i];
                AppendLine(sb, indent + 2, $"Lane[{i}]: Id={lane.Id}, LaneHeight={lane.LaneHeight?.Val}");
            }
        }

        // Dump Mixer if present
        if (deviceChain.Mixer != null)
        {
            DumpMixer(deviceChain.Mixer, sb, indent + 1);
        }

        // Dump devices array
        if (deviceChain.Devices != null && deviceChain.Devices.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Devices Count: {deviceChain.Devices.Count}");
            for (int i = 0; i < deviceChain.Devices.Count; i++)
            {
                var dev = deviceChain.Devices[i];
                DumpDevice(dev, sb, indent + 2, i);
            }
        }
        else
        {
            AppendLine(sb, indent + 1, "[No Devices]");
        }

        // ClipEnvelopeChooserViewState, Routing, Sequencers, etc.
        if (deviceChain.ClipEnvelopeChooserViewState != null)
        {
            AppendLine(sb, indent + 1, "-- ClipEnvelopeChooserViewState --");
            AppendLine(sb, indent + 2, $"SelectedDevice: {deviceChain.ClipEnvelopeChooserViewState.SelectedDevice?.Val}");
            AppendLine(sb, indent + 2, $"SelectedEnvelope: {deviceChain.ClipEnvelopeChooserViewState.SelectedEnvelope?.Val}");
            AppendLine(sb, indent + 2, $"PreferModulationVisible: {deviceChain.ClipEnvelopeChooserViewState.PreferModulationVisible?.Val}");
        }

        if (deviceChain.AudioInputRouting != null)
        {
            DumpRouting("AudioInputRouting", deviceChain.AudioInputRouting, sb, indent + 1);
        }
        if (deviceChain.AudioOutputRouting != null)
        {
            DumpRouting("AudioOutputRouting", deviceChain.AudioOutputRouting, sb, indent + 1);
        }
        if (deviceChain.MidiInputRouting != null)
        {
            DumpRouting("MidiInputRouting", deviceChain.MidiInputRouting, sb, indent + 1);
        }
        if (deviceChain.MidiOutputRouting != null)
        {
            DumpRouting("MidiOutputRouting", deviceChain.MidiOutputRouting, sb, indent + 1);
        }

        // MainSequencer
        if (deviceChain.MainSequencer != null)
        {
            DumpMainSequencer(deviceChain.MainSequencer, sb, indent + 1);
        }
        // FreezeSequencer
        if (deviceChain.FreezeSequencer != null)
        {
            DumpFreezeSequencer(deviceChain.FreezeSequencer, sb, indent + 1);
        }
    }

    private static void DumpMainSequencer(MainSequencer mainSequencer, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- MainSequencer --");
        AppendLine(sb, indent + 1, $"LomId: {mainSequencer.LomId?.Val}");
        AppendLine(sb, indent + 1, $"IsExpanded: {mainSequencer.IsExpanded?.Val}");
        // more fields...
        if (mainSequencer.ClipTimeable?.ArrangerAutomation != null)
        {
            DumpArrangerAutomation(mainSequencer.ClipTimeable.ArrangerAutomation, sb, indent + 1);
        }

        if (mainSequencer.Sample?.ArrangerAutomation != null)
        {
            DumpArrangerAutomation(mainSequencer.Sample.ArrangerAutomation, sb, indent + 1);
        }
    }

    private static void DumpFreezeSequencer(FreezeSequencer freezeSequencer, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- FreezeSequencer --");
        AppendLine(sb, indent + 1, $"LomId: {freezeSequencer.LomId?.Val}");
        AppendLine(sb, indent + 1, $"IsExpanded: {freezeSequencer.IsExpanded?.Val}");
        // more fields...
        if (freezeSequencer.Sample?.ArrangerAutomation != null)
        {
            DumpArrangerAutomation(freezeSequencer.Sample.ArrangerAutomation, sb, indent + 1);
        }

        // ClipSlotList
        if (freezeSequencer.ClipSlotList != null && freezeSequencer.ClipSlotList.Count > 0)
        {
            AppendLine(sb, indent + 1, $"ClipSlotList ({freezeSequencer.ClipSlotList.Count}):");
            for (int i = 0; i < freezeSequencer.ClipSlotList.Count; i++)
            {
                DumpClipSlot(freezeSequencer.ClipSlotList[i], sb, indent + 2, i);
            }
        }
    }

    private static void DumpArrangerAutomation(ArrangerAutomation arrangerAutomation, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- ArrangerAutomation --");
        if (arrangerAutomation.Events != null)
        {
            AppendLine(sb, indent + 1, $"Events Count: {arrangerAutomation.Events.Count}");
            for (int i = 0; i < arrangerAutomation.Events.Count; i++)
            {
                DumpClip(arrangerAutomation.Events[i], sb, indent + 2, i);
            }
        }
        if (arrangerAutomation.AutomationTransformViewState != null)
        {
            DumpAutomationTransformViewState(arrangerAutomation.AutomationTransformViewState, sb, indent + 1);
        }
    }

    private static void DumpControllerLayoutCustomization(ControllerLayoutCustomization clc, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- ControllerLayoutCustomization --");
        AppendLine(sb, indent + 1, $"PitchClassSource: {clc.PitchClassSource?.Val}");
        AppendLine(sb, indent + 1, $"OctaveSource: {clc.OctaveSource?.Val}");
        AppendLine(sb, indent + 1, $"KeyNoteTarget: {clc.KeyNoteTarget?.Val}");
        AppendLine(sb, indent + 1, $"StepSize: {clc.StepSize?.Val}");
        AppendLine(sb, indent + 1, $"OctaveEvery: {clc.OctaveEvery?.Val}");
        AppendLine(sb, indent + 1, $"AllowedKeys: {clc.AllowedKeys?.Val}");
        AppendLine(sb, indent + 1, $"FillerKeysMapTo: {clc.FillerKeysMapTo?.Val}");
    }

    private static void DumpMixer(Mixer mixer, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- Mixer --");
        AppendLine(sb, indent + 1, $"LomId: {mixer.LomId?.Val}");
        AppendLine(sb, indent + 1, $"IsExpanded: {mixer.IsExpanded?.Val}");
        AppendLine(sb, indent + 1, $"SoloSink: {mixer.SoloSink?.Val}");
        AppendLine(sb, indent + 1, $"PanMode: {mixer.PanMode?.Val}");

        // Print volume, pan, sends, tempo, etc.
        if (mixer.Volume != null)
        {
            AppendLine(sb, indent + 1, $"Volume = {mixer.Volume.Manual?.Val}");
        }
        if (mixer.Pan != null)
        {
            AppendLine(sb, indent + 1, $"Pan = {mixer.Pan.Manual?.Val}");
        }
        if (mixer.Tempo != null)
        {
            AppendLine(sb, indent + 1, $"Tempo = {mixer.Tempo.Manual?.Val}");
        }
        if (mixer.Sends != null && mixer.Sends.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Sends: {mixer.Sends.Count}");
            for (int i = 0; i < mixer.Sends.Count; i++)
            {
                var send = mixer.Sends[i];
                AppendLine(sb, indent + 2, $"Send[{i}] = {send.Manual?.Val}");
            }
        }
    }

    private static void DumpDevice(Device device, StringBuilder sb, int indent, int index)
    {
        AppendLine(sb, indent, $"-- Device[{index}] --");
        AppendLine(sb, indent + 1, $"Id: {device.Id?.Val}");
        AppendLine(sb, indent + 1, $"LomId: {device.LomId?.Val}");
        AppendLine(sb, indent + 1, $"IsExpanded: {device.IsExpanded?.Val}");
        AppendLine(sb, indent + 1, $"UserName: {device.UserName?.Val}");
        AppendLine(sb, indent + 1, $"LastPresetRef: {device.LastPresetRef?.Val}");
        AppendLine(sb, indent + 1, $"Annotation: {device.Annotation?.Val}");
        if (device.Parameters != null && device.Parameters.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Parameters: {device.Parameters.Count}");
            for (int i = 0; i < device.Parameters.Count; i++)
            {
                var param = device.Parameters[i];
                AppendLine(sb, indent + 2, $"Param[{i}] Id={param.Id?.Val} Value={param.Value?.Val}");
            }
        }
    }

    private static void DumpRouting(string label, Routing routing, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, $"-- {label} --");
        AppendLine(sb, indent + 1, $"Target: {routing.Target?.Val}");
        AppendLine(sb, indent + 1, $"UpperDisplayString: {routing.UpperDisplayString?.Val}");
        AppendLine(sb, indent + 1, $"LowerDisplayString: {routing.LowerDisplayString?.Val}");
        // etc. MpeSettings if needed
    }

    private static void DumpAutomationTransformViewState(AutomationTransformViewState atvs, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- AutomationTransformViewState --");
        AppendLine(sb, indent + 1, $"IsTransformPending: {atvs.IsTransformPending?.Val}");
        // TimeAndValueTransforms
        if (atvs.TimeAndValueTransforms != null)
        {
            AppendLine(sb, indent + 1, "[TimeAndValueTransforms is present]");
        }
    }

    private static void DumpClip(Clip clip, StringBuilder sb, int indent, int clipIndex)
    {
        if (clip == null)
        {
            AppendLine(sb, indent, $"[Clip {clipIndex} is null]");
            return;
        }

        AppendLine(sb, indent, $"-- Clip[{clipIndex}] {clip.GetType().Name} --");
        AppendLine(sb, indent + 1, $"Id: {clip.Id}");
        AppendLine(sb, indent + 1, $"Time: {clip.Time}");
        AppendLine(sb, indent + 1, $"Name: {clip.Name?.Val}");
        AppendLine(sb, indent + 1, $"Color: {clip.Color?.Val}");
        AppendLine(sb, indent + 1, $"IsWarped: {clip.IsWarped?.Val}");
        AppendLine(sb, indent + 1, $"Loop On: {clip.Loop?.LoopOn?.Val}");

        // AudioClip vs MidiClip specifics
        if (clip is AudioClip audioClip)
        {
            DumpAudioClip(audioClip, sb, indent + 1);
        }
        else if (clip is MidiClip midiClip)
        {
            DumpMidiClip(midiClip, sb, indent + 1);
        }
    }

    private static void DumpAudioClip(AudioClip audioClip, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- AudioClip --");
        if (audioClip.SampleRef != null)
        {
            DumpSampleRef(audioClip.SampleRef, sb, indent + 1);
        }
        if (audioClip.WarpMarkers != null && audioClip.WarpMarkers.Count > 0)
        {
            AppendLine(sb, indent + 1, $"WarpMarkers: {audioClip.WarpMarkers.Count}");
            for (int i = 0; i < audioClip.WarpMarkers.Count; i++)
            {
                var wm = audioClip.WarpMarkers[i];
                AppendLine(sb, indent + 2, $"WarpMarker[{i}] Id={wm.Id}, SecTime={wm.SecTime?.Val}, BeatTime={wm.BeatTime?.Val}");
            }
        }
        AppendLine(sb, indent + 1, $"WarpMode: {audioClip.WarpMode?.Val}");
        AppendLine(sb, indent + 1, $"HiQ: {audioClip.HiQ?.Val}");
        AppendLine(sb, indent + 1, $"PitchCoarse: {audioClip.PitchCoarse?.Val}");
        AppendLine(sb, indent + 1, $"PitchFine: {audioClip.PitchFine?.Val}");
        // etc. for other AudioClip-specific fields
    }

    private static void DumpMidiClip(MidiClip midiClip, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- MidiClip --");
        AppendLine(sb, indent + 1, $"BankSelectCoarse: {midiClip.BankSelectCoarse?.Val}");
        AppendLine(sb, indent + 1, $"BankSelectFine: {midiClip.BankSelectFine?.Val}");
        AppendLine(sb, indent + 1, $"ProgramChange: {midiClip.ProgramChange?.Val}");
        AppendLine(sb, indent + 1, $"IsInKey: {midiClip.IsInKey?.Val}");

        // KeyTracks
        if (midiClip.KeyTracks != null && midiClip.KeyTracks.Count > 0)
        {
            AppendLine(sb, indent + 1, $"KeyTracks ({midiClip.KeyTracks.Count}):");
            for (int i = 0; i < midiClip.KeyTracks.Count; i++)
            {
                var kt = midiClip.KeyTracks[i];
                AppendLine(sb, indent + 2, $"KeyTrack[{i}] Id={kt.Id}, MidiKey={kt.MidiKey?.Val}");
                if (kt.Notes != null && kt.Notes.Count > 0)
                {
                    AppendLine(sb, indent + 3, $"Notes ({kt.Notes.Count}):");
                    for (int n = 0; n < kt.Notes.Count; n++)
                    {
                        var note = kt.Notes[n];
                        AppendLine(sb, indent + 4,
                            $"Note[{n}] Time={note.Time} Dur={note.Duration} Vel={note.Velocity} OffVel={note.OffVelocity} Prob={note.Probability} Enabled={note.IsEnabled}");
                    }
                }
            }
        }
        // etc. for other MidiClip fields
    }

    private static void DumpClipSlot(ClipSlot slot, StringBuilder sb, int indent, int slotIndex)
    {
        AppendLine(sb, indent, $"-- ClipSlot[{slotIndex}] --");
        AppendLine(sb, indent + 1, $"Id: {slot.Id}");
        AppendLine(sb, indent + 1, $"LomId: {slot.LomId?.Val}");
        AppendLine(sb, indent + 1, $"HasStop: {slot.HasStop?.Val}");
        AppendLine(sb, indent + 1, $"NeedRefreeze: {slot.NeedRefreeze?.Val}");

        if (slot.ClipData?.Clip != null)
        {
            DumpClip(slot.ClipData.Clip, sb, indent + 1, slotIndex);
        }
    }

    private static void DumpScene(Scene scene, StringBuilder sb, int indent, int sceneIndex)
    {
        AppendLine(sb, indent, $"-- Scene [{sceneIndex}] --");
        AppendLine(sb, indent + 1, $"Id: {scene.Id}");
        AppendLine(sb, indent + 1, $"Name: {scene.Name?.Val}");
        AppendLine(sb, indent + 1, $"Color: {scene.Color?.Val}");
        AppendLine(sb, indent + 1, $"Tempo: {scene.Tempo?.Val}");
        AppendLine(sb, indent + 1, $"IsTempoEnabled: {scene.IsTempoEnabled?.Val}");
        AppendLine(sb, indent + 1, $"TimeSignatureId: {scene.TimeSignatureId?.Val}");
        AppendLine(sb, indent + 1, $"IsTimeSignatureEnabled: {scene.IsTimeSignatureEnabled?.Val}");
        AppendLine(sb, indent + 1, $"Annotation: {scene.Annotation?.Val}");
        AppendLine(sb, indent + 1, $"LomId: {scene.LomId?.Val}");

        // FollowAction
        if (scene.FollowAction != null)
        {
            AppendLine(sb, indent + 1, $"-- FollowAction --");
            AppendLine(sb, indent + 2, $"FollowTime: {scene.FollowAction.FollowTime?.Val}");
            AppendLine(sb, indent + 2, $"IsLinked: {scene.FollowAction.IsLinked?.Val}");
            AppendLine(sb, indent + 2, $"LoopIterations: {scene.FollowAction.LoopIterations?.Val}");
            AppendLine(sb, indent + 2, $"FollowActionA: {scene.FollowAction.FollowActionA?.Val}");
            AppendLine(sb, indent + 2, $"FollowActionB: {scene.FollowAction.FollowActionB?.Val}");
            AppendLine(sb, indent + 2, $"FollowChanceA: {scene.FollowAction.FollowChanceA?.Val}");
            AppendLine(sb, indent + 2, $"FollowChanceB: {scene.FollowAction.FollowChanceB?.Val}");
            AppendLine(sb, indent + 2, $"JumpIndexA: {scene.FollowAction.JumpIndexA?.Val}");
            AppendLine(sb, indent + 2, $"JumpIndexB: {scene.FollowAction.JumpIndexB?.Val}");
            AppendLine(sb, indent + 2, $"FollowActionEnabled: {scene.FollowAction.FollowActionEnabled?.Val}");
        }

        // ClipSlotsListWrapper
        if (scene.ClipSlotsListWrapper != null)
        {
            AppendLine(sb, indent + 1, $"ClipSlotsListWrapper LomId: {scene.ClipSlotsListWrapper.LomId}");
        }
    }

    private static void DumpTransport(Transport transport, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- Transport --");
        AppendLine(sb, indent + 1, $"PhaseNudgeTempo: {transport.PhaseNudgeTempo?.Val}");
        AppendLine(sb, indent + 1, $"LoopOn: {transport.LoopOn?.Val}");
        AppendLine(sb, indent + 1, $"LoopStart: {transport.LoopStart?.Val}");
        AppendLine(sb, indent + 1, $"LoopLength: {transport.LoopLength?.Val}");
        AppendLine(sb, indent + 1, $"LoopIsSongStart: {transport.LoopIsSongStart?.Val}");
        AppendLine(sb, indent + 1, $"CurrentTime: {transport.CurrentTime?.Val}");
        AppendLine(sb, indent + 1, $"PunchIn: {transport.PunchIn?.Val}");
        AppendLine(sb, indent + 1, $"PunchOut: {transport.PunchOut?.Val}");
        AppendLine(sb, indent + 1, $"MetronomeTickDuration: {transport.MetronomeTickDuration?.Val}");
        AppendLine(sb, indent + 1, $"DrawMode: {transport.DrawMode?.Val}");
    }

    private static void DumpLocator(Locator locator, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- Locator --");
        AppendLine(sb, indent + 1, $"Time: {locator.Time?.Val}");
        AppendLine(sb, indent + 1, $"Name: {locator.Name?.Val}");
        AppendLine(sb, indent + 1, $"Annotation: {locator.Annotation?.Val}");
    }

    private static void DumpGrid(Grid grid, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- Grid --");
        AppendLine(sb, indent + 1, $"FixedNumerator: {grid.FixedNumerator?.Val}");
        AppendLine(sb, indent + 1, $"FixedDenominator: {grid.FixedDenominator?.Val}");
        AppendLine(sb, indent + 1, $"GridIntervalPixel: {grid.GridIntervalPixel?.Val}");
        AppendLine(sb, indent + 1, $"Ntoles: {grid.Ntoles?.Val}");
        AppendLine(sb, indent + 1, $"SnapToGrid: {grid.SnapToGrid?.Val}");
        AppendLine(sb, indent + 1, $"Fixed: {grid.Fixed?.Val}");
    }

    private static void DumpScaleInformation(ScaleInformation scaleInfo, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- ScaleInformation --");
        AppendLine(sb, indent + 1, $"RootNote: {scaleInfo.RootNote?.Val}");
        AppendLine(sb, indent + 1, $"Name: {scaleInfo.Name?.Val}");
    }

    private static void DumpTimeSelection(TimeSelection timeSelection, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- TimeSelection --");
        AppendLine(sb, indent + 1, $"AnchorTime: {timeSelection.AnchorTime?.Val}");
        AppendLine(sb, indent + 1, $"OtherTime: {timeSelection.OtherTime?.Val}");
    }

    private static void DumpSequencerNavigator(SequencerNavigator nav, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- SequencerNavigator --");
        if (nav.BeatTimeHelper != null)
        {
            AppendLine(sb, indent + 1, $"CurrentZoom: {nav.BeatTimeHelper.CurrentZoom?.Val}");
        }
        if (nav.ScrollerPos != null)
        {
            AppendLine(sb, indent + 1, $"ScrollerPos X={nav.ScrollerPos.X?.Val}, Y={nav.ScrollerPos.Y?.Val}");
        }
        if (nav.ClientSize != null)
        {
            AppendLine(sb, indent + 1, $"ClientSize X={nav.ClientSize.X?.Val}, Y={nav.ClientSize.Y?.Val}");
        }
    }

    private static void DumpViewStates(ViewStates vs, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- ViewStates --");
        AppendLine(sb, indent + 1, $"MixerInArrangement: {vs.MixerInArrangement?.Val}");
        AppendLine(sb, indent + 1, $"ArrangerMixerIO: {vs.ArrangerMixerIO?.Val}");
        // ... etc. for all fields in ViewStates
        AppendLine(sb, indent + 1, $"SessionShowOverView: {vs.SessionShowOverView?.Val}");
        AppendLine(sb, indent + 1, $"ArrangerMixer: {vs.ArrangerMixer?.Val}");
    }

    private static void DumpSampleRef(SampleRef sampleRef, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- SampleRef --");
        AppendLine(sb, indent + 1, $"RelativePathType: {sampleRef.FileRef?.RelativePathType?.Val}");
        AppendLine(sb, indent + 1, $"RelativePath: {sampleRef.FileRef?.RelativePath?.Val}");
        AppendLine(sb, indent + 1, $"Path: {sampleRef.FileRef?.Path?.Val}");
        AppendLine(sb, indent + 1, $"LastModDate: {sampleRef.LastModDate?.Val}");
        // etc.
    }

    private static void DumpGroovePool(GroovePool groovePool, StringBuilder sb, int indent)
    {
        AppendLine(sb, indent, "-- GroovePool --");
        AppendLine(sb, indent + 1, $"LomId: {groovePool.LomId}");
        if (groovePool.Grooves != null && groovePool.Grooves.Count > 0)
        {
            AppendLine(sb, indent + 1, $"Grooves Count: {groovePool.Grooves.Count}");
            for (int i = 0; i < groovePool.Grooves.Count; i++)
            {
                DumpGroove(groovePool.Grooves[i], sb, indent + 2, i);
            }
        }
    }

    private static void DumpGroove(Groove groove, StringBuilder sb, int indent, int grooveIndex)
    {
        AppendLine(sb, indent, $"Groove[{grooveIndex}]: Name={groove.Name?.Val}");
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