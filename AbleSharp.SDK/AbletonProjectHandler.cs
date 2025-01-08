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

    public static MidiTrack CreateBlankMidiTrack(string id, string name)
    {
        return new MidiTrack
        {
            Id = id,
            LomId = new Value<int> { Val = 0 },
            LomIdView = new Value<int> { Val = 0 },
            IsContentSelectedInDocument = new Value<bool> { Val = false },
            PreferredContentViewMode = new Value<int> { Val = 0 },
            TrackDelay = new TrackDelay
            {
                Value = new Value<decimal> { Val = 0 },
                IsValueSampleBased = new Value<bool> { Val = false }
            },
            Name = new TrackName
            {
                EffectiveName = new Value<string> { Val = name },
                UserName = new Value<string> { Val = "" },
                Annotation = new Value<string> { Val = "" },
                MemorizedFirstClipName = new Value<string> { Val = "" }
            },
            Color = new Value<string> { Val = "12" },
            AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<Envelope>() },
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = true },
            DevicesListWrapper = new DevicesListWrapper { LomId = new Value<int> { Val = 0 } },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = new Value<int> { Val = 0 } },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = new TakeLanes
            {
                LaneCollection = new List<TakeLane>(),
                AreTakeLanesFolded = new Value<bool> { Val = true }
            },
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            PitchbendRange = new Value<int> { Val = 96 }
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
            TrackDelay = new TrackDelay
            {
                Value = new Value<decimal> { Val = 0 },
                IsValueSampleBased = new Value<bool> { Val = false }
            },
            Name = new TrackName
            {
                EffectiveName = new Value<string> { Val = name },
                UserName = new Value<string> { Val = "" },
                Annotation = new Value<string> { Val = "" },
                MemorizedFirstClipName = new Value<string> { Val = "" }
            },
            Color = new Value<string> { Val = "13" },
            AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<Envelope>() },
            TrackGroupId = new Value<int> { Val = -1 },
            TrackUnfolded = new Value<bool> { Val = true },
            DevicesListWrapper = new DevicesListWrapper { LomId = new Value<int> { Val = 0 } },
            ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = new Value<int> { Val = 0 } },
            ViewData = new Value<string> { Val = "{}" },
            TakeLanes = new TakeLanes
            {
                LaneCollection = new List<TakeLane>(),
                AreTakeLanesFolded = new Value<bool> { Val = true }
            },
            LinkedTrackGroupId = new Value<int> { Val = -1 },
            SavedPlayingSlot = new Value<int> { Val = -1 },
            SavedPlayingOffset = new Value<decimal> { Val = 0 },
            Freeze = new Value<bool> { Val = false },
            VelocityDetail = new Value<int> { Val = 0 },
            NeedArrangerRefreeze = new Value<bool> { Val = true },
            PostProcessFreezeClips = new Value<int> { Val = 0 },
            DeviceChain = new DeviceChain
            {
                AutomationLanes = new AutomationLanes
                {
                    Lanes = new List<AutomationLane>
                    {
                        new AutomationLane
                        {
                            Id = new Value<string> { Val = "0" },
                            SelectedDevice = new Value<int> { Val = 1 },
                            SelectedEnvelope = new Value<int> { Val = 0 },
                            IsContentSelectedInDocument = new Value<bool> { Val = false },
                            LaneHeight = new Value<decimal> { Val = 68 }
                        }
                    },
                    AreAdditionalAutomationLanesFolded = new Value<bool> { Val = false }
                },
                ClipEnvelopeChooserViewState = new ClipEnvelopeChooserViewState
                {
                    SelectedDevice = new Value<int> { Val = 1 },
                    SelectedEnvelope = new Value<int> { Val = 0 },
                    PreferModulationVisible = new Value<bool> { Val = false }
                },
                AudioInputRouting = new AudioInputRouting
                {
                    Target = new Value<string> { Val = "AudioIn/External/S0" },
                    UpperDisplayString = new Value<string> { Val = "Ext. In" },
                    LowerDisplayString = new Value<string> { Val = "1/2" },
                    MpeSettings = new MpeSettings
                    {
                        ZoneType = new Value<int> { Val = 0 },
                        FirstNoteChannel = new Value<int> { Val = 1 },
                        LastNoteChannel = new Value<int> { Val = 15 }
                    }
                },
                MidiInputRouting = new MidiInputRouting
                {
                    Target = new Value<string> { Val = "MidiIn/External.All/-1" },
                    UpperDisplayString = new Value<string> { Val = "Ext: All Ins" },
                    LowerDisplayString = new Value<string> { Val = "" },
                    MpeSettings = new MpeSettings
                    {
                        ZoneType = new Value<int> { Val = 0 },
                        FirstNoteChannel = new Value<int> { Val = 1 },
                        LastNoteChannel = new Value<int> { Val = 15 }
                    }
                },
                AudioOutputRouting = new AudioOutputRouting
                {
                    Target = new Value<string> { Val = "AudioOut/None" },
                    UpperDisplayString = new Value<string> { Val = "Sends Only" },
                    LowerDisplayString = new Value<string> { Val = "" },
                    MpeSettings = new MpeSettings
                    {
                        ZoneType = new Value<int> { Val = 0 },
                        FirstNoteChannel = new Value<int> { Val = 1 },
                        LastNoteChannel = new Value<int> { Val = 15 }
                    }
                },
                MidiOutputRouting = new MidiOutputRouting
                {
                    Target = new Value<string> { Val = "MidiOut/None" },
                    UpperDisplayString = new Value<string> { Val = "None" },
                    LowerDisplayString = new Value<string> { Val = "" },
                    MpeSettings = new MpeSettings
                    {
                        ZoneType = new Value<int> { Val = 0 },
                        FirstNoteChannel = new Value<int> { Val = 1 },
                        LastNoteChannel = new Value<int> { Val = 15 }
                    }
                }
            }
        };
    }

    public static AbletonProject CreateBlankProject()
    {
        return new AbletonProject
        {
            MajorVersion = 5,
            MinorVersion = "11.0_11300",
            SchemaChangeCount = 3,
            Creator = "Ableton Live 11.3.21",
            Revision = "5ac24cad7c51ea0671d49e6b4885371f15b57c1e",
            LiveSet = new LiveSet
            {
                NextPointeeId = new Value<int> { Val = 22155 },
                OverwriteProtectionNumber = new Value<int> { Val = 2819 },
                LomId = new Value<int> { Val = 0 },
                LomIdView = new Value<int> { Val = 0 },

                Tracks =
                [
                    CreateBlankMidiTrack("12", "SITSEM"),
                    CreateBlankAudioTrack("13", "BITSEM"),
                ],

                MainTrack = new MainTrack
                {
                    LomId = new Value<int> { Val = 0 },
                    LomIdView = new Value<int> { Val = 0 },
                    IsContentSelectedInDocument = new Value<bool> { Val = false },
                    PreferredContentViewMode = new Value<int> { Val = 0 },
                    TrackDelay = new TrackDelay
                    {
                        Value = new Value<decimal> { Val = 0 },
                        IsValueSampleBased = new Value<bool> { Val = false }
                    },
                    Name = new TrackName
                    {
                        EffectiveName = new Value<string> { Val = "Master" },
                        UserName = new Value<string> { Val = "" },
                        Annotation = new Value<string> { Val = "" },
                        MemorizedFirstClipName = new Value<string> { Val = "" }
                    },
                    Color = new Value<string> { Val = "24" },
                    AutomationEnvelopes = new AutomationEnvelopes
                    {
                        Envelopes =
                        [
                            new Envelope
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
                                    }
                                }
                            },

                            new Envelope
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
                                            Value = 120
                                        }
                                    }
                                }
                            }
                        ]
                    },
                    TrackGroupId = new Value<int> { Val = -1 },
                    TrackUnfolded = new Value<bool> { Val = false },
                    DevicesListWrapper = new DevicesListWrapper { LomId = 0 },
                    ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 },
                    ViewData = new Value<string> { Val = "{}" },
                    TakeLanes = new TakeLanes
                    {
                        LaneCollection = new List<TakeLane>(),
                        AreTakeLanesFolded = new Value<bool> { Val = true }
                    },
                    LinkedTrackGroupId = new Value<int> { Val = -1 }
                },

                PreHearTrack = new PreHearTrack
                {
                    LomId = new Value<int> { Val = 0 },
                    LomIdView = new Value<int> { Val = 0 },
                    IsContentSelectedInDocument = new Value<bool> { Val = false },
                    PreferredContentViewMode = new Value<int> { Val = 0 },
                    TrackDelay = new TrackDelay
                    {
                        Value = new Value<decimal> { Val = 0 },
                        IsValueSampleBased = new Value<bool> { Val = false }
                    },
                    Name = new TrackName
                    {
                        EffectiveName = new Value<string> { Val = "Master" },
                        UserName = new Value<string> { Val = "" },
                        Annotation = new Value<string> { Val = "" },
                        MemorizedFirstClipName = new Value<string> { Val = "" }
                    },
                    Color = new Value<string> { Val = "-1" },
                    AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<Envelope>() },
                    TrackGroupId = new Value<int> { Val = -1 },
                    TrackUnfolded = new Value<bool> { Val = false },
                    DevicesListWrapper = new DevicesListWrapper { LomId = new Value<int> { Val = 0 } },
                    ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = new Value<int> { Val = 0 } },
                    ViewData = new Value<string> { Val = "{}" },
                    TakeLanes = new TakeLanes
                    {
                        LaneCollection = new List<TakeLane>(),
                        AreTakeLanesFolded = new Value<bool> { Val = true }
                    },
                    LinkedTrackGroupId = new Value<int> { Val = -1 }
                },

                Scenes = new List<Scene>
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
                        ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = new Value<int> { Val = 0 } }
                    }
                    // Add more scenes if needed
                },

                SendsPre = [],

                Transport = new Transport
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
                },

                ViewStates = new ViewStates
                {
                    SessionIO = new Value<int> { Val = 1 },
                    SessionSends = new Value<int> { Val = 1 },
                    SessionReturns = new Value<int> { Val = 1 },
                    SessionShowOverView = new Value<int> { Val = 0 },
                    ArrangerIO = new Value<int> { Val = 1 },
                    ArrangerReturns = new Value<int> { Val = 1 },
                    ArrangerMixer = new Value<int> { Val = 1 },
                    ArrangerTrackDelay = new Value<int> { Val = 0 },
                    ArrangerShowOverView = new Value<int> { Val = 1 }
                }
            }
        };
    }
}