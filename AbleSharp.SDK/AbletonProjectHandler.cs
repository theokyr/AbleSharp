namespace AbleSharp.SDK;

using System.IO.Compression;
using System.Xml.Serialization;
using AbleSharp.Lib;

public static class AbletonProjectHandler
{
    public static AbletonProject LoadFromFile(string filePath)
    {
        // Read the gzipped file into memory
        using var fileStream = File.OpenRead(filePath);
        using var memStream = new MemoryStream();
        fileStream.CopyTo(memStream);
        memStream.Position = 0;

        // Try to decompress as gzip
        try
        {
            using var gzipStream = new GZipStream(memStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Position = 0;

            // Deserialize the XML
            var serializer = new XmlSerializer(typeof(AbletonProject));
            return (AbletonProject)serializer.Deserialize(decompressedStream)!;
        }
        catch (InvalidDataException)
        {
            // If gzip decompression fails, try to deserialize directly (might be already decompressed)
            memStream.Position = 0;
            var serializer = new XmlSerializer(typeof(AbletonProject));
            return (AbletonProject)serializer.Deserialize(memStream)!;
        }
    }

    public static void SaveToFile(AbletonProject abletonProject, string filePath)
    {
        using var memStream = new MemoryStream();
        var serializer = new XmlSerializer(typeof(AbletonProject));

        // First serialize to XML
        serializer.Serialize(memStream, abletonProject);
        memStream.Position = 0;

        // Then compress with gzip
        using var fileStream = File.Create(filePath);
        using var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal);
        memStream.CopyTo(gzipStream);
    }

    public static AbletonProject CreateBlankProject()
    {
        IdGenerator.Reset();

        var project = new AbletonProject
        {
            MajorVersion = 5,
            MinorVersion = "12.0_12049",
            SchemaChangeCount = 12,
            Creator = "Ableton Live 12.0.10",
            Revision = "518b0e8f662095a813fbfe2191c405929dce7c4f",
            LiveSet = CreateBlankLiveSet() 
        };

        int highestIdUsed = IdGenerator.GetLastId(); 
        if (project.LiveSet.NextPointeeId.Val <= highestIdUsed)
        {
            project.LiveSet.NextPointeeId.Val = new Value<int> { Val = highestIdUsed + 1 };
        }

        return project;
    }

    private static LiveSet CreateBlankLiveSet()
    {
        return new LiveSet
        {
            NextPointeeId = new Value<int> { Val = 22155 },
            OverwriteProtectionNumber = new Value<int> { Val = 2819 },
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            Tracks = new List<Track>
            {
                CreateBlankMidiTrack("12", "Generated Midi Track"),
                CreateBlankAudioTrack("13", "Generated Audio Track"),
            },
            MainTrack = CreateMainTrack(),
            PreHearTrack = CreatePreHearTrack(),
            SendsPre = new List<SendsPre>(),
            Scenes = CreateDefaultScenes(),
            Transport = CreateDefaultTransport(),
            ViewStates = CreateDefaultViewStates(),
            AutomationMode = new Value<bool> { Val = false },
            SnapAutomationToGrid = new Value<bool> { Val = true },
            ArrangementOverdub = new Value<bool> { Val = false },
            GlobalQuantisation = new Value<int> { Val = 4 },
            AutoQuantisation = new Value<int> { Val = 0 },
            ChooserBar = new Value<int> { Val = 0 },
            Annotation = new Value<string> { Val = "" },
            SoloOrPflSavedValue = new Value<bool> { Val = true },
            SoloInPlace = new Value<bool> { Val = true },
            CrossfadeCurve = new Value<int> { Val = 2 },
            LatencyCompensation = new Value<int> { Val = 2 },
            HighlightedTrackIndex = new Value<int> { Val = 0 },
            ColorSequenceIndex = new Value<int> { Val = 0 },
            MidiFoldIn = new Value<bool> { Val = false },
            MidiFoldMode = new Value<int> { Val = 0 },
            MultiClipFocusMode = new Value<bool> { Val = false },
            MultiClipLoopBarHeight = new Value<decimal> { Val = 0 },
            MidiPrelisten = new Value<bool> { Val = false },
            AccidentalSpellingPreference = new Value<int> { Val = 3 },
            PreferFlatRootNote = new Value<bool> { Val = false },
            UseWarperLegacyHiQMode = new Value<bool> { Val = false },
            SmpteFormat = new Value<int> { Val = 0 },
            InKey = new Value<bool> { Val = false },
            TracksListWrapper = new TracksListWrapper { LomId = 0 },
            VisibleTracksListWrapper = new TracksListWrapper { LomId = 0 },
            ReturnTracksListWrapper = new TracksListWrapper { LomId = 0 },
            ScenesListWrapper = new TracksListWrapper { LomId = 0 },
            CuePointsListWrapper = new TracksListWrapper { LomId = 0 },
            GroovePool = new GroovePool
            {
                LomId = 0,
                Grooves = new List<Groove>()
            },
            SignalModulationsTop = new object(),
            SequencerNavigator = new SequencerNavigator
            {
                BeatTimeHelper = new BeatTimeHelper
                {
                    CurrentZoom = new Value<decimal> { Val = 0.254945M } 
                },
                ScrollerPos = new ScrollerPos
                {
                    X = new Value<int> { Val = 0 },
                    Y = new Value<int> { Val = 0 }
                },
                ClientSize = new ClientSize
                {
                    X = new Value<int> { Val = 528 },
                    Y = new Value<int> { Val = 437 }
                }
            },
            Locators = new List<Locator>()
        };
    }

    public static MainTrack CreateMainTrack()
    {
        return new MainTrack
        {
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsContentSelectedInDocument = new Value<bool> { Val = false },
            PreferredContentViewMode = new Value<int> { Val = 0 },
            TrackDelay = CreateDefaultTrackDelay(),
            Name = CreateTrackName("Main"),
            Color = new Value<string> { Val = "18" },
            AutomationEnvelopes = CreateMainTrackAutomationEnvelopes(),
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = false },
            DevicesListWrapper = new DevicesListWrapper { LomId = 0 },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = CreateDefaultTakeLanes(),
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            DeviceChain = CreateMainTrackDeviceChain()
        };
    }

    private static AutomationEnvelopes CreateMainTrackAutomationEnvelopes()
    {
        return new AutomationEnvelopes
        {
            Envelopes = new List<AutomationEnvelope>
            {
                new AutomationEnvelope
                {
                    Id = "0",
                    EnvelopeTarget = new EnvelopeTarget
                    {
                        PointeeId = new Value<string> { Val = "10" }
                    },
                    Automation = new Automation
                    {
                        Events = new List<AutomationEventBase>
                        {
                            new EnumEvent
                            {
                                Id = "0",
                                Time = -63072000,
                                Value = 201
                            }
                        },
                        AutomationTransformViewState = CreateDefaultAutomationTransformViewState()
                    }
                },
                new AutomationEnvelope
                {
                    Id = "1",
                    EnvelopeTarget = new EnvelopeTarget
                    {
                        PointeeId = new Value<string> { Val = "8" }
                    },
                    Automation = new Automation
                    {
                        Events = new List<AutomationEventBase>
                        {
                            new FloatEvent
                            {
                                Id = "0",
                                Time = -63072000,
                                Value = 112
                            }
                        },
                        AutomationTransformViewState = CreateDefaultAutomationTransformViewState()
                    }
                }
            }
        };
    }

    private static AutomationTransformViewState CreateDefaultAutomationTransformViewState()
    {
        return new AutomationTransformViewState
        {
            IsTransformPending = new Value<bool> { Val = false },
            TimeAndValueTransforms = new TimeAndValueTransforms()
        };
    }

    private static TrackDelay CreateDefaultTrackDelay()
    {
        return new TrackDelay
        {
            Value = new Value<decimal> { Val = 0 },
            IsValueSampleBased = new Value<bool> { Val = false }
        };
    }

    private static TrackName CreateTrackName(string name, string annotation = "", string memorizedFirstClipName = "")
    {
        return new TrackName
        {
            EffectiveName = new Value<string> { Val = name },
            UserName = new Value<string> { Val = name },
            Annotation = new Value<string> { Val = annotation },
            MemorizedFirstClipName = new Value<string> { Val = memorizedFirstClipName }
        };
    }

    private static TakeLanes CreateDefaultTakeLanes()
    {
        return new TakeLanes
        {
            LaneCollection = new List<TakeLane>(),
            AreTakeLanesFolded = new Value<bool> { Val = true }
        };
    }

    public static PreHearTrack CreatePreHearTrack()
    {
        return new PreHearTrack
        {
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsContentSelectedInDocument = new Value<bool> { Val = false },
            PreferredContentViewMode = new Value<int> { Val = 0 },
            TrackDelay = CreateDefaultTrackDelay(),
            Name = CreateTrackName("Master"),
            Color = new Value<string> { Val = "-1" },
            AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<AutomationEnvelope>() },
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = false },
            DevicesListWrapper = new DevicesListWrapper { LomId = 0 },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = CreateDefaultTakeLanes(),
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            DeviceChain = CreateDefaultDeviceChain()
        };
    }

    public static MidiTrack CreateBlankMidiTrack(string id, string name)
    {
        return new MidiTrack
        {
            Id = id,
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsContentSelectedInDocument = new Value<bool> { Val = false },
            PreferredContentViewMode = new Value<int> { Val = 0 },
            TrackDelay = CreateDefaultTrackDelay(),
            Name = CreateTrackName(name),
            Color = new Value<string> { Val = "6" },
            AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<AutomationEnvelope>() },
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = true },
            DevicesListWrapper = new DevicesListWrapper { LomId = 0 },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = CreateDefaultTakeLanes(),
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            DeviceChain = CreateDefaultDeviceChain(includeSequencers: true),
            ReWireDeviceMidiTargetId = new Value<int> { Val = 0 },
            PitchbendRange = new Value<int> { Val = 96 },
            IsTuned = new Value<bool> { Val = true },
            SavedPlayingSlot = new Value<int> { Val = -1 },
            SavedPlayingOffset = new Value<decimal> { Val = 0 },
            Freeze = new Value<bool> { Val = false },
            VelocityDetail = new Value<int> { Val = 0 },
            NeedArrangerRefreeze = new Value<bool> { Val = true },
            PostProcessFreezeClips = new Value<int> { Val = 0 },
            ControllerLayoutRemoteable = new Value<int> { Val = 0 },
            ControllerLayoutCustomization = CreateDefaultControllerLayoutCustomization()
        };
    }

    public static AudioTrack CreateBlankAudioTrack(string id, string name)
    {
        return new AudioTrack
        {
            Id = id,
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsContentSelectedInDocument = new Value<bool> { Val = false },
            PreferredContentViewMode = new Value<int> { Val = 0 },
            TrackDelay = CreateDefaultTrackDelay(),
            Name = CreateTrackName(name),
            Color = new Value<string> { Val = "13" },
            AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<AutomationEnvelope>() },
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = true },
            DevicesListWrapper = new DevicesListWrapper { LomId = 0 },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = CreateDefaultTakeLanes(),
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            DeviceChain = CreateDefaultDeviceChain(includeSequencers: true),
            SavedPlayingSlot = new Value<int> { Val = -1 },
            SavedPlayingOffset = new Value<decimal> { Val = 0 },
            Freeze = new Value<bool> { Val = false },
            VelocityDetail = new Value<int> { Val = 0 },
            NeedArrangerRefreeze = new Value<bool> { Val = true },
            PostProcessFreezeClips = new Value<int> { Val = 0 }
        };
    }

    private static DeviceChain CreateDefaultDeviceChain(bool includeSequencers = false)
    {
        var chain = new DeviceChain
        {
            AutomationLanes = CreateDefaultAutomationLanes(),
            ClipEnvelopeChooserViewState = CreateDefaultClipEnvelopeChooserViewState(),
            AudioInputRouting = CreateDefaultAudioInputRouting(),
            AudioOutputRouting = CreateDefaultAudioOutputRouting(),
            MidiInputRouting = CreateDefaultMidiInputRouting(),
            MidiOutputRouting = CreateDefaultMidiOutputRouting(),
            Mixer = CreateDefaultMixer(),
            Devices = new List<Device>(),
            SignalModulations = new SignalModulations()
        };

        if (includeSequencers)
        {
            chain.MainSequencer = CreateDefaultMainSequencer();
            chain.FreezeSequencer = CreateDefaultFreezeSequencer();
        }

        return chain;
    }

    private static MainSequencer CreateDefaultMainSequencer()
    {
        return new MainSequencer
        {
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsExpanded = new Value<bool> { Val = true },
            On = CreateDefaultOn(),
            ModulationSourceCount = new Value<int> { Val = 0 },
            ParametersListWrapper = new ParametersListWrapper { LomId = 0 },
            Pointee = new Pointee { Id = IdGenerator.GetNextId() },
            LastSelectedTimeableIndex = new Value<int> { Val = 0 },
            LastSelectedClipEnvelopeIndex = new Value<int> { Val = 0 },
            LastPresetRef = new LastPresetRef { Value = "" },
            LockedScripts = new LockedScripts(),
            IsFolded = new Value<bool> { Val = false },
            ShouldShowPresetName = new Value<bool> { Val = false },
            UserName = new Value<string> { Val = "" },
            Annotation = new Value<string> { Val = "" },
            SourceContext = new SourceContext { Value = "" },
            ClipSlotList = CreateDefaultClipSlotList(),
            MonitoringEnum = new Value<MonitoringEnum> { Val = MonitoringEnum.In },
            KeepRecordMonitoringLatency = new Value<bool> { Val = true },
            Sample = new Sample
            {
                ArrangerAutomation = CreateDefaultArrangerAutomation()
            },
            ClipTimeable = new ClipTimeable
            {
                ArrangerAutomation = CreateDefaultArrangerAutomation()
            },
            Recorder = CreateDefaultRecorder()
        };
    }

    private static DeviceChain CreateMainTrackDeviceChain()
    {
        var chain = CreateDefaultDeviceChain();
        chain.AudioOutputRouting.Target = new Value<string> { Val = "AudioOut/External/S0" };
        chain.AudioOutputRouting.UpperDisplayString = new Value<string> { Val = "Ext. Out" };
        chain.Mixer.Tempo = CreateTempo(133.7m);
        return chain;
    }

    private static AutomationLanes CreateDefaultAutomationLanes()
    {
        return new AutomationLanes
        {
            Lanes = new List<AutomationLane>
            {
                new AutomationLane
                {
                    Id = "0",
                    SelectedDevice = new Value<int> { Val = 1 },
                    SelectedEnvelope = new Value<int> { Val = 0 },
                    IsContentSelectedInDocument = new Value<bool> { Val = false },
                    LaneHeight = new Value<decimal> { Val = 68 }
                }
            },
            AreAdditionalAutomationLanesFolded = new Value<bool> { Val = false }
        };
    }

    private static ClipEnvelopeChooserViewState CreateDefaultClipEnvelopeChooserViewState()
    {
        return new ClipEnvelopeChooserViewState
        {
            SelectedDevice = new Value<int> { Val = 1 },
            SelectedEnvelope = new Value<int> { Val = 0 },
            PreferModulationVisible = new Value<bool> { Val = false }
        };
    }

    private static AudioInputRouting CreateDefaultAudioInputRouting()
    {
        return new AudioInputRouting
        {
            Target = new Value<string> { Val = "AudioIn/External/S0" },
            UpperDisplayString = new Value<string> { Val = "Ext. In" },
            LowerDisplayString = new Value<string> { Val = "1/2" },
            MpeSettings = CreateDefaultMpeSettings()
        };
    }

    private static MidiInputRouting CreateDefaultMidiInputRouting()
    {
        return new MidiInputRouting
        {
            Target = new Value<string> { Val = "MidiIn/External.All/-1" },
            UpperDisplayString = new Value<string> { Val = "Ext: All Ins" },
            LowerDisplayString = new Value<string> { Val = "" },
            MpeSettings = CreateDefaultMpeSettings()
        };
    }

    private static MidiOutputRouting CreateDefaultMidiOutputRouting()
    {
        return new MidiOutputRouting
        {
            Target = new Value<string> { Val = "MidiOut/None" },
            UpperDisplayString = new Value<string> { Val = "None" },
            LowerDisplayString = new Value<string> { Val = "" },
            MpeSettings = CreateDefaultMpeSettings()
        };
    }

    private static MpeSettings CreateDefaultMpeSettings()
    {
        return new MpeSettings
        {
            ZoneType = new Value<int> { Val = 0 },
            FirstNoteChannel = new Value<int> { Val = 1 },
            LastNoteChannel = new Value<int> { Val = 15 }
        };
    }

    private static Mixer CreateDefaultMixer()
    {
        return new Mixer
        {
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsExpanded = new Value<bool> { Val = true },
            On = new On
            {
                LomId = new Value<int> { Val = 0 },
                Manual = new Value<bool> { Val = true },
                AutomationTarget = CreateAutomationTarget(),
                MidiCCOnOffThresholds = new MidiCCOnOffThresholds
                {
                    Min = new Value<int> { Val = 64 },
                    Max = new Value<int> { Val = 127 }
                }
            },
            ModulationSourceCount = new Value<int> { Val = 0 },
            ParametersListWrapper = new ParametersListWrapper { LomId = 0 },
            Pointee = new Pointee { Id = IdGenerator.GetNextId() },
            LastSelectedTimeableIndex = new Value<int> { Val = 0 },
            LastSelectedClipEnvelopeIndex = new Value<int> { Val = 0 },
            LastPresetRef = new LastPresetRef { Value = "" },
            LockedScripts = new LockedScripts(),
            IsFolded = new Value<bool> { Val = false },
            ShouldShowPresetName = new Value<bool> { Val = false },
            UserName = new Value<string> { Val = "" },
            Annotation = new Value<string> { Val = "" },
            SourceContext = new SourceContext { Value = "" },
            SendsListWrapper = new SendsListWrapper { LomId = 0 },
            Speaker = CreateDefaultSpeaker(),
            SoloSink = new Value<bool> { Val = false },
            PanMode = new Value<int> { Val = 0 },
            Pan = CreateDefaultPan(),
            Volume = CreateVolume(),
            ViewStateSesstionTrackWidth = new Value<decimal> { Val = 93 },
            CrossFadeState = new CrossFadeState
            {
                LomId = new Value<int> { Val = 0 },
                Manual = new Value<decimal> { Val = 1 },
                AutomationTarget = CreateAutomationTarget()
            }
        };
    }

    private static Speaker CreateDefaultSpeaker()
    {
        return new Speaker
        {
            LomId = new Value<int> { Val = 0 },
            Manual = new Value<bool> { Val = true },
            AutomationTarget = CreateAutomationTarget(),
            MidiCCOnOffThresholds = new MidiCCOnOffThresholds
            {
                Min = new Value<int> { Val = 64 },
                Max = new Value<int> { Val = 127 }
            }
        };
    }

    private static Pan CreateDefaultPan()
    {
        return new Pan
        {
            LomId = new Value<int> { Val = 0 },
            Manual = new Value<decimal> { Val = 0 },
            MidiControllerRange = new MidiControllerRange
            {
                Min = new Value<decimal> { Val = -1 },
                Max = new Value<decimal> { Val = 1 }
            },
            AutomationTarget = CreateAutomationTarget(),
            ModulationTarget = CreateModulationTarget()
        };
    }

    private static Volume CreateVolume(decimal value = 1)
    {
        return new Volume
        {
            LomId = new Value<int> { Val = 0 },
            Manual = new Value<decimal> { Val = value },
            MidiControllerRange = new MidiControllerRange
            {
                Min = new Value<decimal> { Val = 0.0003162277571M },
                Max = new Value<decimal> { Val = 1.99526238M }
            },
            AutomationTarget = CreateAutomationTarget(),
            ModulationTarget = CreateModulationTarget()
        };
    }

    private static Tempo CreateTempo(decimal value = 120)
    {
        return new Tempo
        {
            LomId = new Value<int> { Val = 0 },
            Manual = new Value<decimal> { Val = value },
            MidiControllerRange = new MidiControllerRange
            {
                Min = new Value<decimal> { Val = 60 },
                Max = new Value<decimal> { Val = 200 }
            },
            AutomationTarget = CreateAutomationTarget(),
            ModulationTarget = CreateModulationTarget()
        };
    }

    private static List<Scene> CreateDefaultScenes()
    {
        return new List<Scene>
        {
            new Scene
            {
                Id = "0",
                FollowAction = new FollowAction
                {
                    FollowTime = new Value<decimal> { Val = 4 },
                    IsLinked = new Value<bool> { Val = true },
                    LoopIterations = new Value<int> { Val = 1 },
                    FollowActionA = new Value<int> { Val = 4 },
                    FollowActionB = new Value<int> { Val = 0 },
                    FollowChanceA = new Value<int> { Val = 100 },
                    FollowChanceB = new Value<int> { Val = 0 },
                    JumpIndexA = new Value<int> { Val = 0 },
                    JumpIndexB = new Value<int> { Val = 0 },
                    FollowActionEnabled = new Value<bool> { Val = false }
                },
                Name = new Value<string> { Val = "" },
                Annotation = new Value<string> { Val = "" },
                Color = new Value<string> { Val = "-1" },
                Tempo = new Value<decimal> { Val = 120 },
                IsTempoEnabled = new Value<bool> { Val = false },
                TimeSignatureId = new Value<string> { Val = "201" },
                IsTimeSignatureEnabled = new Value<bool> { Val = false },
                LomId = new Value<int> { Val = 0 },
                ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 }
            }
        };
    }

    private static Transport CreateDefaultTransport()
    {
        return new Transport
        {
            PhaseNudgeTempo = new Value<decimal> { Val = 10 },
            LoopOn = new Value<bool> { Val = false },
            LoopStart = new Value<decimal> { Val = 8 },
            LoopLength = new Value<decimal> { Val = 16 },
            LoopIsSongStart = new Value<bool> { Val = false },
            CurrentTime = new Value<decimal> { Val = 0 },
            PunchIn = new Value<bool> { Val = false },
            PunchOut = new Value<bool> { Val = false },
            MetronomeTickDuration = new Value<decimal> { Val = 0 },
            DrawMode = new Value<bool> { Val = false }
        };
    }

    private static ViewStates CreateDefaultViewStates()
    {
        return new ViewStates
        {
            SessionIO = new Value<int> { Val = 1 },
            SessionSends = new Value<int> { Val = 1 },
            SessionReturns = new Value<int> { Val = 1 },
            SessionShowOverView = new Value<int> { Val = 0 },
            ArrangerIO = new Value<int> { Val = 1 },
            ArrangerReturns = new Value<int> { Val = 1 },
            ArrangerShowOverView = new Value<int> { Val = 1 },
            ArrangerTrackDelay = new Value<int> { Val = 0 },
            ArrangerMixer = new Value<int> { Val = 1 }
        };
    }

    private static ModulationTarget CreateModulationTarget()
    {
        return new ModulationTarget
        {
            Id = IdGenerator.GetNextId(),
            LockEnvelope = new Value<int> { Val = 0 }
        };
    }

    private static AutomationTarget CreateAutomationTarget()
    {
        return new AutomationTarget
        {
            Id = IdGenerator.GetNextId(),
            LockEnvelope = new Value<int> { Val = 0 }
        };
    }

    private static FreezeSequencer CreateDefaultFreezeSequencer()
    {
        return new FreezeSequencer
        {
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsExpanded = new Value<bool> { Val = true },
            On = CreateDefaultOn(),
            ModulationSourceCount = new Value<int> { Val = 0 },
            ParametersListWrapper = new ParametersListWrapper { LomId = 0 },
            Pointee = new Pointee { Id = IdGenerator.GetNextId() },
            LastSelectedTimeableIndex = new Value<int> { Val = 0 },
            LastSelectedClipEnvelopeIndex = new Value<int> { Val = 0 },
            LastPresetRef = new LastPresetRef { Value = "" },
            LockedScripts = new LockedScripts(),
            IsFolded = new Value<bool> { Val = false },
            ShouldShowPresetName = new Value<bool> { Val = false },
            UserName = new Value<string> { Val = "" },
            Annotation = new Value<string> { Val = "" },
            SourceContext = new SourceContext { Value = "" },
            ClipSlotList = CreateDefaultClipSlotList(),
            MonitoringEnum = new Value<MonitoringEnum> { Val = MonitoringEnum.In },
            KeepRecordMonitoringLatency = new Value<bool> { Val = true },
            Sample = new Sample
            {
                ArrangerAutomation = CreateDefaultArrangerAutomation()
            },
            VolumeModulationTarget = CreateModulationTarget(),
            TranspositionModulationTarget = CreateModulationTarget(),
            TransientEnvelopeModulationTarget = CreateModulationTarget(),
            GrainSizeModulationTarget = CreateModulationTarget(),
            FluxModulationTarget = CreateModulationTarget(),
            SampleOffsetModulationTarget = CreateModulationTarget(),
            ComplexProFormantsModulationTarget = CreateModulationTarget(),
            ComplexProEnvelopeModulationTarget = CreateModulationTarget(),
            PitchViewScrollPosition = new Value<decimal> { Val = -1073741824 },
            SampleOffsetModulationScrollPosition = new Value<decimal> { Val = -1073741824 },
            Recorder = CreateDefaultRecorder()
        };
    }

    private static List<ClipSlot> CreateDefaultClipSlotList()
    {
        var clipSlots = new List<ClipSlot>();
        for (int i = 0; i < 8; i++)
        {
            clipSlots.Add(new ClipSlot
            {
                Id = i.ToString(),
                LomId = new Value<int> { Val = 0 },
                ClipData = new ClipSlotValue { Value = "" },
                HasStop = new Value<bool> { Val = true },
                NeedRefreeze = new Value<bool> { Val = true }
            });
        }

        return clipSlots;
    }

    private static ArrangerAutomation CreateDefaultArrangerAutomation()
    {
        return new ArrangerAutomation
        {
            Events = new List<AutomationEvent>(),
            AutomationTransformViewState = new AutomationTransformViewState
            {
                IsTransformPending = new Value<bool> { Val = false },
                TimeAndValueTransforms = new TimeAndValueTransforms()
            }
        };
    }


    private static ControllerLayoutCustomization CreateDefaultControllerLayoutCustomization()
    {
        return new ControllerLayoutCustomization
        {
            PitchClassSource = new Value<int> { Val = 0 },
            OctaveSource = new Value<int> { Val = 2 },
            KeyNoteTarget = new Value<int> { Val = 60 },
            StepSize = new Value<int> { Val = 1 },
            OctaveEvery = new Value<int> { Val = 12 },
            AllowedKeys = new Value<int> { Val = 0 },
            FillerKeysMapTo = new Value<int> { Val = 0 }
        };
    }

    private static AudioOutputRouting CreateDefaultAudioOutputRouting()
    {
        return new AudioOutputRouting
        {
            Target = new Value<string> { Val = "AudioOut/Main" },
            UpperDisplayString = new Value<string> { Val = "Master" },
            LowerDisplayString = new Value<string> { Val = "" },
            MpeSettings = CreateDefaultMpeSettings()
        };
    }

    private static On CreateDefaultOn()
    {
        return new On
        {
            LomId = new Value<int> { Val = 0 },
            Manual = new Value<bool> { Val = true },
            AutomationTarget = new AutomationTarget
            {
                Id = IdGenerator.GetNextId(),
                LockEnvelope = new Value<int> { Val = 0 }
            },
            MidiCCOnOffThresholds = new MidiCCOnOffThresholds
            {
                Min = new Value<int> { Val = 64 },
                Max = new Value<int> { Val = 127 }
            }
        };
    }

    private static Recorder CreateDefaultRecorder()
    {
        return new Recorder
        {
            IsArmed = new Value<bool> { Val = false },
            TakeCounter = new Value<int> { Val = 1 }
        };
    }
}

public static class IdGenerator
{
    private static int _nextId = 1;

    public static string GetNextId()
    {
        return (_nextId++).ToString();
    }

    public static int GetLastId()
    {
        return _nextId;
    }

    public static void Reset()
    {
        _nextId = 1;
    }
}