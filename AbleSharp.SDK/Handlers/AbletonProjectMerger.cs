using AbleSharp.Lib;
using AbleSharp.SDK.Factories;
using AbleSharp.SDK.Utils;

namespace AbleSharp.SDK;

public static class AbletonProjectMerger
{
    /// <summary>
    /// Merges multiple Ableton Live projects into a single new project.
    /// </summary>
    /// <param name="projects">Collection of projects to merge</param>
    /// <returns>A new AbletonProject containing all tracks from the input projects</returns>
    public static AbletonProject MergeProjects(IEnumerable<AbletonProject> projects)
    {
        if (projects == null || !projects.Any())
            throw new ArgumentException("At least one project must be provided for merging");

        // Prepare ID Generator
        IdGenerator.Reset();

        // Create a new blank project as our target
        var mergedProject = AbletonProjectFactory.CreateBlankProject();

        // Clear any default tracks that might have been created
        mergedProject.LiveSet.Tracks.Clear();

        // Track the highest nextPointeeId across all projects
        var highestPointeeId = mergedProject.LiveSet.NextPointeeId.Val;

        foreach (var sourceProject in projects)
        {
            if (sourceProject?.LiveSet?.Tracks == null)
                continue;

            // Update highest PointeeId if needed
            if (sourceProject.LiveSet.NextPointeeId.Val > highestPointeeId)
                highestPointeeId = sourceProject.LiveSet.NextPointeeId.Val;

            // Add each track from the source project
            foreach (var track in sourceProject.LiveSet.Tracks)
                try
                {
                    // Deep clone the track to avoid reference issues
                    var clonedTrack = CloneTrack(track);

                    if (clonedTrack != null)
                        // Add to our merged project
                        mergedProject.LiveSet.Tracks.Add(clonedTrack);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other tracks
                    Console.WriteLine($"Warning: Failed to clone track {track.Name?.EffectiveName?.Val}. Error: {ex.Message}");
                    continue;
                }
        }

        // Update the NextPointeeId to be higher than any existing IDs
        mergedProject.LiveSet.NextPointeeId = new Value<int> { Val = highestPointeeId + 100 };

        // Update ViewData and other settings
        UpdateViewSettings(mergedProject);

        // Validations
        ValidatePointeeIds(mergedProject);

        return mergedProject;
    }

    /// <summary>
    /// Creates a deep clone of a track, including all its properties and child tracks
    /// </summary>
    private static Track CloneTrack(Track sourceTrack)
    {
        // Create new instance of the same track type
        Track newTrack;

        try
        {
            switch (sourceTrack)
            {
                case MidiTrack _:
                    newTrack = new MidiTrack();
                    break;
                case AudioTrack _:
                    newTrack = new AudioTrack();
                    break;
                case GroupTrack _:
                    newTrack = new GroupTrack();
                    break;
                default:
                    Console.WriteLine($"Warning: Skipping unsupported track type: {sourceTrack.GetType().Name}");
                    return null;
            }

            newTrack.Id = IdGenerator.GetNextId();
            newTrack.LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) };
            newTrack.LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) };

            newTrack.IsContentSelectedInDocument = sourceTrack.IsContentSelectedInDocument;
            newTrack.PreferredContentViewMode = sourceTrack.PreferredContentViewMode;

            // Copy track delay
            if (sourceTrack.TrackDelay != null)
                newTrack.TrackDelay = new TrackDelay
                {
                    Value = sourceTrack.TrackDelay.Value,
                    IsValueSampleBased = sourceTrack.TrackDelay.IsValueSampleBased
                };

            // Copy track name
            if (sourceTrack.Name != null)
                newTrack.Name = new TrackName
                {
                    EffectiveName = sourceTrack.Name.EffectiveName,
                    UserName = sourceTrack.Name.UserName,
                    Annotation = sourceTrack.Name.Annotation,
                    MemorizedFirstClipName = sourceTrack.Name.MemorizedFirstClipName
                };

            // Copy color
            newTrack.Color = sourceTrack.Color;

            // Copy automation envelopes
            if (sourceTrack.AutomationEnvelopes?.Envelopes != null)
                newTrack.AutomationEnvelopes = new AutomationEnvelopes
                {
                    Envelopes = sourceTrack.AutomationEnvelopes.Envelopes.Select(CloneAutomationEnvelope).ToList()
                };

            // Copy track group info
            newTrack.TrackGroupId = sourceTrack.TrackGroupId;
            newTrack.TrackUnfolded = sourceTrack.TrackUnfolded;
            newTrack.LinkedTrackGroupId = sourceTrack.LinkedTrackGroupId;

            // Copy device chain
            if (sourceTrack.DeviceChain != null) newTrack.DeviceChain = CloneDeviceChain(sourceTrack.DeviceChain);

            // Copy specific properties based on track type
            if (sourceTrack is FreezableTrack sourceFreezable && newTrack is FreezableTrack newFreezable)
            {
                newFreezable.Freeze = sourceFreezable.Freeze;
                newFreezable.NeedArrangerRefreeze = sourceFreezable.NeedArrangerRefreeze;
                newFreezable.PostProcessFreezeClips = sourceFreezable.PostProcessFreezeClips;
            }

            if (sourceTrack is MidiTrack sourceMidi && newTrack is MidiTrack newMidi)
            {
                newMidi.PitchbendRange = sourceMidi.PitchbendRange;
                newMidi.IsTuned = sourceMidi.IsTuned;
                newMidi.SavedPlayingSlot = sourceMidi.SavedPlayingSlot;
                newMidi.SavedPlayingOffset = sourceMidi.SavedPlayingOffset;
                newMidi.VelocityDetail = sourceMidi.VelocityDetail;
                newMidi.ControllerLayoutRemoteable = sourceMidi.ControllerLayoutRemoteable;

                if (sourceMidi.ControllerLayoutCustomization != null)
                    newMidi.ControllerLayoutCustomization = new ControllerLayoutCustomization
                    {
                        PitchClassSource = sourceMidi.ControllerLayoutCustomization.PitchClassSource,
                        OctaveSource = sourceMidi.ControllerLayoutCustomization.OctaveSource,
                        KeyNoteTarget = sourceMidi.ControllerLayoutCustomization.KeyNoteTarget,
                        StepSize = sourceMidi.ControllerLayoutCustomization.StepSize,
                        OctaveEvery = sourceMidi.ControllerLayoutCustomization.OctaveEvery,
                        AllowedKeys = sourceMidi.ControllerLayoutCustomization.AllowedKeys,
                        FillerKeysMapTo = sourceMidi.ControllerLayoutCustomization.FillerKeysMapTo
                    };
            }

            if (sourceTrack is AudioTrack sourceAudio && newTrack is AudioTrack newAudio)
            {
                newAudio.SavedPlayingSlot = sourceAudio.SavedPlayingSlot;
                newAudio.SavedPlayingOffset = sourceAudio.SavedPlayingOffset;
                newAudio.VelocityDetail = sourceAudio.VelocityDetail;
            }

            if (sourceTrack is GroupTrack sourceGroup && newTrack is GroupTrack newGroup)
                if (sourceGroup.Slots != null)
                    newGroup.Slots = sourceGroup.Slots.Select(slot => new GroupTrackSlot
                    {
                        Id = IdGenerator.GetNextId(),
                        LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) }
                    }).ToList();

            return newTrack;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cloning track: {ex.Message}");
            return null;
        }
    }

    private static AutomationEnvelope CloneAutomationEnvelope(AutomationEnvelope source)
    {
        if (source == null) return null;

        var newEnvelope = new AutomationEnvelope
        {
            Id = IdGenerator.GetNextId(),
            EnvelopeTarget = source.EnvelopeTarget != null
                ? new EnvelopeTarget
                {
                    PointeeId = source.EnvelopeTarget.PointeeId
                }
                : null
        };

        if (source.Automation != null)
            newEnvelope.Automation = new Automation
            {
                Events = source.Automation.Events?.ToList(),
                AutomationTransformViewState = source.Automation.AutomationTransformViewState != null
                    ? new AutomationTransformViewState
                    {
                        IsTransformPending = source.Automation.AutomationTransformViewState.IsTransformPending,
                        TimeAndValueTransforms = new TimeAndValueTransforms()
                    }
                    : null
            };

        return newEnvelope;
    }

    private static DeviceChain CloneDeviceChain(DeviceChain source)
    {
        if (source == null) return null;

        var pointeeId = IdGenerator.GetNextId();

        var newChain = new DeviceChain
        {
            AutomationLanes = CloneAutomationLanes(source.AutomationLanes),
            ClipEnvelopeChooserViewState = CloneClipEnvelopeChooserViewState(source.ClipEnvelopeChooserViewState),
            Mixer = CloneMixer(source.Mixer, pointeeId), // Pass pointee ID
            Devices = source.Devices?.Select(CloneDevice).ToList(),
            SignalModulations = new SignalModulations(),
            MainSequencer = CloneMainSequencer(source.MainSequencer),
            FreezeSequencer = CloneFreezeSequencer(source.FreezeSequencer)
        };

        try
        {
            newChain.AudioInputRouting = (AudioInputRouting)CloneRouting(source.AudioInputRouting, typeof(AudioInputRouting));
            newChain.AudioOutputRouting = (AudioOutputRouting)CloneRouting(source.AudioOutputRouting, typeof(AudioOutputRouting));
            newChain.MidiInputRouting = (MidiInputRouting)CloneRouting(source.MidiInputRouting, typeof(MidiInputRouting));
            newChain.MidiOutputRouting = (MidiOutputRouting)CloneRouting(source.MidiOutputRouting, typeof(MidiOutputRouting));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error cloning routing settings: {ex.Message}. Using default routing.");

            // Set up default routing
            newChain.AudioInputRouting = new AudioInputRouting
            {
                Target = new Value<string> { Val = "AudioIn/External/S0" },
                UpperDisplayString = new Value<string> { Val = "Ext. In" },
                LowerDisplayString = new Value<string> { Val = "1/2" }
            };
            newChain.AudioOutputRouting = new AudioOutputRouting
            {
                Target = new Value<string> { Val = "AudioOut/Main" },
                UpperDisplayString = new Value<string> { Val = "Master" }
            };
            newChain.MidiInputRouting = new MidiInputRouting
            {
                Target = new Value<string> { Val = "MidiIn/External.All/-1" },
                UpperDisplayString = new Value<string> { Val = "Ext: All Ins" }
            };
            newChain.MidiOutputRouting = new MidiOutputRouting
            {
                Target = new Value<string> { Val = "MidiOut/None" },
                UpperDisplayString = new Value<string> { Val = "None" }
            };
        }

        return newChain;
    }

    private static MainSequencer CloneMainSequencer(MainSequencer source)
    {
        if (source == null) return null;

        var pointeeId = IdGenerator.GetNextId();

        var newSequencer = new MainSequencer
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            IsExpanded = source.IsExpanded,
            On = CloneOn(source.On),
            ModulationSourceCount = source.ModulationSourceCount,
            ParametersListWrapper = source.ParametersListWrapper,
            Pointee = new Pointee { Id = pointeeId },
            LastSelectedTimeableIndex = source.LastSelectedTimeableIndex,
            LastSelectedClipEnvelopeIndex = source.LastSelectedClipEnvelopeIndex,
            LastPresetRef = source.LastPresetRef,
            LockedScripts = new LockedScripts(),
            IsFolded = source.IsFolded,
            ShouldShowPresetName = source.ShouldShowPresetName,
            UserName = source.UserName,
            Annotation = source.Annotation,
            SourceContext = source.SourceContext,
            ClipSlotList = CloneClipSlotList(source.ClipSlotList),
            MonitoringEnum = source.MonitoringEnum,
            KeepRecordMonitoringLatency = source.KeepRecordMonitoringLatency,
            ClipTimeable = CloneClipTimeable(source.ClipTimeable),
            Sample = CloneSample(source.Sample),
            Recorder = CloneRecorder(source.Recorder),
            MidiControllers = source.MidiControllers
        };

        return newSequencer;
    }

    private static FreezeSequencer CloneFreezeSequencer(FreezeSequencer source)
    {
        if (source == null) return null;

        var pointeeId = IdGenerator.GetNextId();

        return new FreezeSequencer
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            IsExpanded = source.IsExpanded,
            On = CloneOn(source.On),
            ModulationSourceCount = source.ModulationSourceCount,
            ParametersListWrapper = source.ParametersListWrapper,
            Pointee = new Pointee { Id = pointeeId },
            LastSelectedTimeableIndex = source.LastSelectedTimeableIndex,
            LastSelectedClipEnvelopeIndex = source.LastSelectedClipEnvelopeIndex,
            LastPresetRef = source.LastPresetRef,
            LockedScripts = new LockedScripts(),
            IsFolded = source.IsFolded,
            ShouldShowPresetName = source.ShouldShowPresetName,
            UserName = source.UserName,
            Annotation = source.Annotation,
            SourceContext = source.SourceContext,
            ClipSlotList = CloneClipSlotList(source.ClipSlotList),
            MonitoringEnum = source.MonitoringEnum,
            KeepRecordMonitoringLatency = source.KeepRecordMonitoringLatency,
            Sample = CloneSample(source.Sample),
            VolumeModulationTarget = CloneModulationTarget(source.VolumeModulationTarget),
            TranspositionModulationTarget = CloneModulationTarget(source.TranspositionModulationTarget),
            TransientEnvelopeModulationTarget = CloneModulationTarget(source.TransientEnvelopeModulationTarget),
            GrainSizeModulationTarget = CloneModulationTarget(source.GrainSizeModulationTarget),
            FluxModulationTarget = CloneModulationTarget(source.FluxModulationTarget),
            SampleOffsetModulationTarget = CloneModulationTarget(source.SampleOffsetModulationTarget),
            ComplexProFormantsModulationTarget = CloneModulationTarget(source.ComplexProFormantsModulationTarget),
            ComplexProEnvelopeModulationTarget = CloneModulationTarget(source.ComplexProEnvelopeModulationTarget),
            PitchViewScrollPosition = source.PitchViewScrollPosition,
            SampleOffsetModulationScrollPosition = source.SampleOffsetModulationScrollPosition,
            Recorder = CloneRecorder(source.Recorder)
        };
    }

    private static AutomationLanes CloneAutomationLanes(AutomationLanes source)
    {
        if (source == null) return null;

        return new AutomationLanes
        {
            Lanes = source.Lanes?.Select(lane => new AutomationLane
            {
                Id = lane.Id,
                SelectedDevice = lane.SelectedDevice,
                SelectedEnvelope = lane.SelectedEnvelope,
                IsContentSelectedInDocument = lane.IsContentSelectedInDocument,
                LaneHeight = lane.LaneHeight
            }).ToList(),
            AreAdditionalAutomationLanesFolded = source.AreAdditionalAutomationLanesFolded
        };
    }

    private static ClipEnvelopeChooserViewState CloneClipEnvelopeChooserViewState(ClipEnvelopeChooserViewState source)
    {
        if (source == null) return null;

        return new ClipEnvelopeChooserViewState
        {
            SelectedDevice = source.SelectedDevice,
            SelectedEnvelope = source.SelectedEnvelope,
            PreferModulationVisible = source.PreferModulationVisible
        };
    }

    private static Routing CloneRouting(Routing source, Type targetType)
    {
        if (source == null) return null;

        // Create the correct type of routing object
        var newRouting = (Routing)Activator.CreateInstance(targetType);

        // Copy the common properties
        newRouting.Target = source.Target;
        newRouting.UpperDisplayString = source.UpperDisplayString;
        newRouting.LowerDisplayString = source.LowerDisplayString;
        newRouting.MpeSettings = source.MpeSettings != null
            ? new MpeSettings
            {
                ZoneType = source.MpeSettings.ZoneType,
                FirstNoteChannel = source.MpeSettings.FirstNoteChannel,
                LastNoteChannel = source.MpeSettings.LastNoteChannel
            }
            : null;

        return newRouting;
    }

    private static Mixer CloneMixer(Mixer source, string pointeeId)
    {
        if (source == null) return null;

        return new Mixer
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            IsExpanded = source.IsExpanded,
            On = CloneOn(source.On),
            ModulationSourceCount = source.ModulationSourceCount,
            ParametersListWrapper = source.ParametersListWrapper,
            Pointee = new Pointee { Id = pointeeId },
            LastSelectedTimeableIndex = source.LastSelectedTimeableIndex,
            LastSelectedClipEnvelopeIndex = source.LastSelectedClipEnvelopeIndex,
            LastPresetRef = source.LastPresetRef,
            LockedScripts = new LockedScripts(),
            IsFolded = source.IsFolded,
            ShouldShowPresetName = source.ShouldShowPresetName,
            UserName = source.UserName,
            Annotation = source.Annotation,
            SourceContext = source.SourceContext,
            Sends = source.Sends?.Select(CloneSend).ToList(),
            Speaker = CloneSpeaker(source.Speaker),
            SoloSink = source.SoloSink,
            PanMode = source.PanMode,
            Pan = ClonePan(source.Pan),
            Volume = CloneVolume(source.Volume),
            ViewStateSesstionTrackWidth = source.ViewStateSesstionTrackWidth,
            CrossFadeState = CloneCrossFadeState(source.CrossFadeState),
            SendsListWrapper = source.SendsListWrapper
        };
    }

    private static Device CloneDevice(Device source)
    {
        if (source == null) return null;

        return new Device
        {
            Id = IdGenerator.GetNextId(),
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            IsExpanded = source.IsExpanded,
            On = CloneOn(source.On),
            ModulationSourceCount = source.ModulationSourceCount,
            ParametersListWrapper = source.ParametersListWrapper,
            LastPresetRef = source.LastPresetRef,
            IsFolded = source.IsFolded,
            ShouldShowPresetName = source.ShouldShowPresetName,
            UserName = source.UserName,
            Annotation = source.Annotation,
            Parameters = source.Parameters?.Select(CloneParameter).ToList()
        };
    }

    private static Parameter CloneParameter(Parameter source)
    {
        if (source == null) return null;

        return new Parameter
        {
            Id = IdGenerator.GetNextId(),
            Value = source.Value,
            MidiControllerRange = source.MidiControllerRange != null
                ? new MidiControllerRange
                {
                    Min = source.MidiControllerRange.Min,
                    Max = source.MidiControllerRange.Max
                }
                : null,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget)
        };
    }

    private static ClipTimeable CloneClipTimeable(ClipTimeable source)
    {
        if (source == null) return null;

        return new ClipTimeable
        {
            ArrangerAutomation = source.ArrangerAutomation != null
                ? new ArrangerAutomation
                {
                    Events = source.ArrangerAutomation.Events?.Select(CloneClip).ToList(),
                    AutomationTransformViewState = source.ArrangerAutomation.AutomationTransformViewState
                }
                : null
        };
    }

    private static Sample CloneSample(Sample source)
    {
        if (source == null) return null;

        return new Sample
        {
            ArrangerAutomation = source.ArrangerAutomation != null
                ? new ArrangerAutomation
                {
                    Events = source.ArrangerAutomation.Events?.Select(CloneClip).ToList(),
                    AutomationTransformViewState = source.ArrangerAutomation.AutomationTransformViewState
                }
                : null
        };
    }

    private static List<ClipSlot> CloneClipSlotList(List<ClipSlot> source)
    {
        if (source == null) return null;

        return source.Select(slot => new ClipSlot
        {
            Id = slot.Id,
            LomId = slot.LomId,
            ClipData = slot.ClipData != null
                ? new ClipSlotValue
                {
                    Value = slot.ClipData.Value,
                    Clip = CloneClip(slot.ClipData.Clip)
                }
                : null,
            HasStop = slot.HasStop,
            NeedRefreeze = slot.NeedRefreeze
        }).ToList();
    }

    private static void UpdateViewSettings(AbletonProject project)
    {
        if (project?.LiveSet == null) return;

        // Update ViewData to show all tracks
        project.LiveSet.ViewData = new Value<string> { Val = "{}" };

        // Ensure proper track indexing
        for (var i = 0; i < project.LiveSet.Tracks.Count; i++)
        {
            var track = project.LiveSet.Tracks[i];
            if (track.DeviceChain?.Mixer != null) track.DeviceChain.Mixer.ViewStateSesstionTrackWidth = new Value<decimal> { Val = 93 };
        }

        // Update ViewStates for optimal visibility
        if (project.LiveSet.ViewStates == null) project.LiveSet.ViewStates = new ViewStates();

        project.LiveSet.ViewStates.MixerInSession = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.SessionIO = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.SessionSends = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.SessionReturns = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.SessionVolume = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.SessionShowOverView = new Value<int> { Val = 0 };
        project.LiveSet.ViewStates.ArrangerIO = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.ArrangerReturns = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.ArrangerVolume = new Value<int> { Val = 1 };
        project.LiveSet.ViewStates.ArrangerShowOverView = new Value<int> { Val = 1 };
    }

    private static On CloneOn(On source)
    {
        if (source == null) return null;

        return new On
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget),
            MidiCCOnOffThresholds = source.MidiCCOnOffThresholds != null
                ? new MidiCCOnOffThresholds
                {
                    Min = source.MidiCCOnOffThresholds.Min,
                    Max = source.MidiCCOnOffThresholds.Max
                }
                : null
        };
    }

    private static ModulationTarget CloneModulationTarget(ModulationTarget source)
    {
        if (source == null) return null;

        return new ModulationTarget
        {
            Id = IdGenerator.GetNextId(),
            LockEnvelope = source.LockEnvelope
        };
    }

    private static Send CloneSend(Send source)
    {
        if (source == null) return null;

        return new Send
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            MidiControllerRange = source.MidiControllerRange != null
                ? new MidiControllerRange
                {
                    Min = source.MidiControllerRange.Min,
                    Max = source.MidiControllerRange.Max
                }
                : null,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget),
            ModulationTarget = CloneModulationTarget(source.ModulationTarget)
        };
    }

    private static Speaker CloneSpeaker(Speaker source)
    {
        if (source == null) return null;

        return new Speaker
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget),
            MidiCCOnOffThresholds = source.MidiCCOnOffThresholds != null
                ? new MidiCCOnOffThresholds
                {
                    Min = source.MidiCCOnOffThresholds.Min,
                    Max = source.MidiCCOnOffThresholds.Max
                }
                : null
        };
    }

    private static Pan ClonePan(Pan source)
    {
        if (source == null) return null;

        return new Pan
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            MidiControllerRange = source.MidiControllerRange != null
                ? new MidiControllerRange
                {
                    Min = source.MidiControllerRange.Min,
                    Max = source.MidiControllerRange.Max
                }
                : null,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget),
            ModulationTarget = CloneModulationTarget(source.ModulationTarget)
        };
    }

    private static Volume CloneVolume(Volume source)
    {
        if (source == null) return null;

        return new Volume
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            MidiControllerRange = source.MidiControllerRange != null
                ? new MidiControllerRange
                {
                    Min = source.MidiControllerRange.Min,
                    Max = source.MidiControllerRange.Max
                }
                : null,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget),
            ModulationTarget = CloneModulationTarget(source.ModulationTarget)
        };
    }

    private static Recorder CloneRecorder(Recorder source)
    {
        if (source == null) return null;

        return new Recorder
        {
            IsArmed = source.IsArmed,
            TakeCounter = source.TakeCounter
        };
    }

    private static CrossFadeState CloneCrossFadeState(CrossFadeState source)
    {
        if (source == null) return null;

        return new CrossFadeState
        {
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            Manual = source.Manual,
            AutomationTarget = CloneAutomationTarget(source.AutomationTarget)
        };
    }

    private static AutomationTarget CloneAutomationTarget(AutomationTarget source)
    {
        if (source == null) return null;

        return new AutomationTarget
        {
            Id = IdGenerator.GetNextId(),
            LockEnvelope = source.LockEnvelope
        };
    }

    private static Clip CloneClip(Clip source)
    {
        if (source == null) return null;

        // Handle different clip types
        if (source is MidiClip midiClip)
            return CloneMidiClip(midiClip);
        if (source is AudioClip audioClip)
            return CloneAudioClip(audioClip);

        // Log warning for unsupported clip types but don't break
        Console.WriteLine($"Warning: Unsupported clip type encountered: {source.GetType().Name}");
        return null;
    }

    private static MidiClip CloneMidiClip(MidiClip source)
    {
        var newClip = new MidiClip
        {
            Id = IdGenerator.GetNextId(),
            Time = source.Time,
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            CurrentStart = source.CurrentStart,
            CurrentEnd = source.CurrentEnd,
            Loop = CloneLoop(source.Loop),
            Name = source.Name,
            Annotation = source.Annotation,
            Color = source.Color,
            LaunchMode = source.LaunchMode,
            LaunchQuantisation = source.LaunchQuantisation,
            TimeSignature = CloneTimeSignature(source.TimeSignature),
            Envelopes = CloneEnvelopes(source.Envelopes),
            ScrollerTimePreserver = source.ScrollerTimePreserver,
            TimeSelection = source.TimeSelection,
            Legato = source.Legato,
            Ram = source.Ram,
            GrooveSettings = source.GrooveSettings,
            Disabled = source.Disabled,
            VelocityAmount = source.VelocityAmount,
            Grid = source.Grid,
            FreezeStart = source.FreezeStart,
            FreezeEnd = source.FreezeEnd,
            IsWarped = source.IsWarped,
            NoteSpellingPreference = source.NoteSpellingPreference,
            BankSelectCoarse = source.BankSelectCoarse,
            BankSelectFine = source.BankSelectFine,
            ProgramChange = source.ProgramChange,
            ScaleInformation = source.ScaleInformation,
            IsInKey = source.IsInKey
        };

        // Deep clone KeyTracks if they exist
        if (source.KeyTracks != null)
            newClip.KeyTracks = source.KeyTracks.Select(kt => new KeyTrack
            {
                Id = IdGenerator.GetNextId(),
                Notes = kt.Notes?.Select(n => new MidiNoteEvent
                {
                    Time = n.Time,
                    Duration = n.Duration,
                    Velocity = n.Velocity,
                    VelocityDeviation = n.VelocityDeviation,
                    OffVelocity = n.OffVelocity,
                    Probability = n.Probability,
                    IsEnabled = n.IsEnabled,
                    NoteId = int.Parse(IdGenerator.GetNextId())
                }).ToList(),
                MidiKey = kt.MidiKey
            }).ToList();

        return newClip;
    }

    private static AudioClip CloneAudioClip(AudioClip source)
    {
        var newClip = new AudioClip
        {
            Id = IdGenerator.GetNextId(),
            Time = source.Time,
            LomId = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            LomIdView = new Value<int> { Val = int.Parse(IdGenerator.GetNextId()) },
            CurrentStart = source.CurrentStart,
            CurrentEnd = source.CurrentEnd,
            Loop = CloneLoop(source.Loop),
            Name = source.Name,
            Annotation = source.Annotation,
            Color = source.Color,
            LaunchMode = source.LaunchMode,
            LaunchQuantisation = source.LaunchQuantisation,
            TimeSignature = CloneTimeSignature(source.TimeSignature),
            Envelopes = CloneEnvelopes(source.Envelopes),
            ScrollerTimePreserver = source.ScrollerTimePreserver,
            TimeSelection = source.TimeSelection,
            Legato = source.Legato,
            Ram = source.Ram,
            GrooveSettings = source.GrooveSettings,
            Disabled = source.Disabled,
            VelocityAmount = source.VelocityAmount,
            Grid = source.Grid,
            FreezeStart = source.FreezeStart,
            FreezeEnd = source.FreezeEnd,
            IsWarped = source.IsWarped,
            WarpMode = source.WarpMode,
            GranularityTones = source.GranularityTones,
            GranularityTexture = source.GranularityTexture,
            FluctuationTexture = source.FluctuationTexture,
            TransientResolution = source.TransientResolution,
            TransientLoopMode = source.TransientLoopMode,
            TransientEnvelope = source.TransientEnvelope,
            ComplexProFormants = source.ComplexProFormants,
            ComplexProEnvelope = source.ComplexProEnvelope,
            Sync = source.Sync,
            HiQ = source.HiQ,
            Fade = source.Fade,
            SampleRef = source.SampleRef, // This is a simple property copy, assuming SampleRef is immutable
            SampleVolume = source.SampleVolume,
            PitchCoarse = source.PitchCoarse,
            PitchFine = source.PitchFine
        };

        // Clone WarpMarkers if they exist
        if (source.WarpMarkers != null)
            newClip.WarpMarkers = source.WarpMarkers.Select(wm => new WarpMarker
            {
                Id = IdGenerator.GetNextId(),
                SecTime = wm.SecTime,
                BeatTime = wm.BeatTime
            }).ToList();

        return newClip;
    }

    private static Loop CloneLoop(Loop source)
    {
        if (source == null) return null;

        return new Loop
        {
            LoopStart = source.LoopStart,
            LoopEnd = source.LoopEnd,
            StartRelative = source.StartRelative,
            LoopOn = source.LoopOn,
            OutMarker = source.OutMarker,
            HiddenLoopStart = source.HiddenLoopStart,
            HiddenLoopEnd = source.HiddenLoopEnd
        };
    }

    private static TimeSignature CloneTimeSignature(TimeSignature source)
    {
        if (source == null) return null;

        return new TimeSignature
        {
            TimeSignatures = source.TimeSignatures?.Select(ts => new RemoteableTimeSignature
            {
                Id = ts.Id,
                Numerator = ts.Numerator,
                Denominator = ts.Denominator,
                Time = ts.Time
            }).ToList()
        };
    }

    private static Envelopes CloneEnvelopes(Envelopes source)
    {
        if (source == null) return null;

        return new Envelopes
        {
            EnvelopeCollection = source.EnvelopeCollection?.Select(e => new ClipEnvelope
            {
                Id = e.Id,
                EnvelopeTarget = e.EnvelopeTarget,
                Automation = e.Automation,
                LoopSlot = e.LoopSlot,
                ScrollerTimePreserver = e.ScrollerTimePreserver
            }).ToList()
        };
    }

    private static void ValidatePointeeIds(AbletonProject project)
    {
        if (project?.LiveSet == null) return;

        // Ensure NextPointeeId is higher than any used ID
        var highestUsedId = IdGenerator.GetLastId();
        if (project.LiveSet.NextPointeeId.Val <= highestUsedId) project.LiveSet.NextPointeeId = new Value<int> { Val = highestUsedId + 100 };
    }
}