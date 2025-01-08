using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using AbleSharp.Lib;

namespace AbleSharp.Lib;

[XmlRoot("Ableton")]
public class AbletonProject
{
    [XmlAttribute] public int MajorVersion { get; set; }

    [XmlAttribute] public string MinorVersion { get; set; }

    [XmlAttribute] public int SchemaChangeCount { get; set; }

    [XmlAttribute] public string Creator { get; set; }

    [XmlAttribute] public string Revision { get; set; }

    [XmlElement("LiveSet")] public LiveSet LiveSet { get; set; }
}

public class LiveSet
{
    public Value<int> NextPointeeId { get; set; }
    public Value<int> OverwriteProtectionNumber { get; set; }
    public Value<int> LomId { get; set; }
    public Value<int> LomIdView { get; set; }

    [XmlArray("Tracks")]
    [XmlArrayItem("MidiTrack", typeof(MidiTrack))]
    [XmlArrayItem("AudioTrack", typeof(AudioTrack))]
    public List<Track> Tracks { get; set; }

    [XmlElement("MasterTrack")] public MainTrack MainTrack { get; set; }

    [XmlElement("PreHearTrack")] public PreHearTrack PreHearTrack { get; set; }

    [XmlArray("Scenes")]
    [XmlArrayItem("Scene")]
    public List<Scene> Scenes { get; set; }

    public Transport Transport { get; set; }
    public List<SendsPre> SendsPre { get; set; }
    public List<Locator> Locators { get; set; }
    public Value<bool> AutomationMode { get; set; }
    public Value<bool> SnapAutomationToGrid { get; set; }
    public Value<bool> ArrangementOverdub { get; set; }
    public Value<int> GlobalQuantisation { get; set; }
    public Value<int> AutoQuantisation { get; set; }
    public Grid Grid { get; set; }
    public ViewData ViewData { get; set; }
    public Value<bool> MidiFoldIn { get; set; }
    public Value<int> MidiFoldMode { get; set; }
    public Value<bool> MultiClipFocusMode { get; set; }
    public Value<decimal> MultiClipLoopBarHeight { get; set; }
    public Value<bool> MidiPrelisten { get; set; }
    public ScaleInformation ScaleInformation { get; set; }
    public Value<bool> InKey { get; set; }
    public Value<int> SmpteFormat { get; set; }
    public TimeSelection TimeSelection { get; set; }
    public SequencerNavigator SequencerNavigator { get; set; }
    public Value<bool> IsContentSplitterOpen { get; set; }
    public Value<bool> IsExpressionSplitterOpen { get; set; }
    public List<ExpressionLane> ExpressionLanes { get; set; }
    public List<ContentLane> ContentLanes { get; set; }
    public ViewStates ViewStates { get; set; }
}

[XmlInclude(typeof(MidiTrack))]
[XmlInclude(typeof(AudioTrack))]
[XmlInclude(typeof(ReturnTrack))]
[XmlInclude(typeof(MainTrack))]
[XmlInclude(typeof(PreHearTrack))]
public abstract class Track
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")] public Value<int> LomIdView { get; set; }

    [XmlElement("IsContentSelectedInDocument")]
    public Value<bool> IsContentSelectedInDocument { get; set; }

    [XmlElement("PreferredContentViewMode")]
    public Value<int> PreferredContentViewMode { get; set; }

    [XmlElement("TrackDelay")] public TrackDelay TrackDelay { get; set; }

    [XmlElement("Name")] public TrackName Name { get; set; }

    [XmlElement("Color")] public Value<string> Color { get; set; }

    [XmlElement("AutomationEnvelopes")] public AutomationEnvelopes AutomationEnvelopes { get; set; }

    [XmlElement("TrackGroupId")] public Value<int> TrackGroupId { get; set; }

    [XmlElement("TrackUnfolded")] public Value<bool> TrackUnfolded { get; set; }

    [XmlElement("DevicesListWrapper")] public DevicesListWrapper DevicesListWrapper { get; set; }

    [XmlElement("ClipSlotsListWrapper")] public ClipSlotsListWrapper ClipSlotsListWrapper { get; set; }

    [XmlElement("ViewData")] public Value<string> ViewData { get; set; }

    [XmlElement("TakeLanes")] public TakeLanes TakeLanes { get; set; }

    [XmlElement("LinkedTrackGroupId")] public Value<int> LinkedTrackGroupId { get; set; }

    [XmlElement("DeviceChain")] public DeviceChain DeviceChain { get; set; }

    [XmlElement("ReWireDeviceMidiTargetId")]
    public Value<int> ReWireDeviceMidiTargetId { get; set; }
}

[XmlType("AudioTrack")]
public class AudioTrack : Track
{
    [XmlElement("SavedPlayingSlot")] public Value<int> SavedPlayingSlot { get; set; }

    [XmlElement("SavedPlayingOffset")] public Value<decimal> SavedPlayingOffset { get; set; }

    [XmlElement("Freeze")] public Value<bool> Freeze { get; set; }

    [XmlElement("VelocityDetail")] public Value<int> VelocityDetail { get; set; }

    [XmlElement("NeedArrangerRefreeze")] public Value<bool> NeedArrangerRefreeze { get; set; }

    [XmlElement("PostProcessFreezeClips")] public Value<int> PostProcessFreezeClips { get; set; }
}

[XmlType("MidiTrack")]
public class MidiTrack : Track
{
    [XmlElement("ReWireSlaveMidiTargetId")]
    public Value<int> ReWireSlaveMidiTargetId { get; set; }

    [XmlElement("PitchbendRange")] public Value<int> PitchbendRange { get; set; }

    [XmlElement("IsTuned")] public Value<bool> IsTuned { get; set; }

    [XmlElement("ControllerLayoutRemoteable")]
    public Value<int> ControllerLayoutRemoteable { get; set; }

    [XmlElement("ControllerLayoutCustomization")]
    public ControllerLayoutCustomization ControllerLayoutCustomization { get; set; }
}

[XmlType("MainTrack")]
public class MainTrack : Track
{
    [XmlElement("Tempo")] public Value<decimal> Tempo { get; set; }

    [XmlElement("TimeSignature")] public TimeSignature TimeSignature { get; set; }
}

[XmlType("ReturnTrack")]
public class ReturnTrack : Track
{
}

[XmlType("PreHearTrack")]
public class PreHearTrack : Track
{
}

public class DeviceChain
{
    [XmlElement("AutomationLanes")] public AutomationLanes AutomationLanes { get; set; }

    [XmlElement("ClipEnvelopeChooserViewState")]
    public ClipEnvelopeChooserViewState ClipEnvelopeChooserViewState { get; set; }

    [XmlElement("AudioInputRouting")] public AudioInputRouting AudioInputRouting { get; set; }

    [XmlElement("AudioOutputRouting")] public AudioOutputRouting AudioOutputRouting { get; set; }

    [XmlElement("MidiInputRouting")] public MidiInputRouting MidiInputRouting { get; set; }

    [XmlElement("MidiOutputRouting")] public MidiOutputRouting MidiOutputRouting { get; set; }

    [XmlElement("Mixer")] public Mixer Mixer { get; set; }

    [XmlElement("MainSequencer")] public MainSequencer MainSequencer { get; set; }

    [XmlElement("FreezeSequencer")] public FreezeSequencer FreezeSequencer { get; set; }

    [XmlArray("Devices")]
    [XmlArrayItem("Device")]
    public List<Device> Devices { get; set; }

    // TODO
    // [XmlArray("SignalModulations")]
    // [XmlArrayItem("SignalModulation")]
    // public List<SignalModulation> SignalModulations { get; set; }
}

[XmlInclude(typeof(MidiClip))]
[XmlInclude(typeof(AudioClip))]
public abstract class Clip
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")] public Value<int> LomIdView { get; set; }

    [XmlElement("CurrentStart")] public Value<decimal> CurrentStart { get; set; }

    [XmlElement("CurrentEnd")] public Value<decimal> CurrentEnd { get; set; }

    [XmlElement("Loop")] public Loop Loop { get; set; }

    [XmlElement("Name")] public Value<string> Name { get; set; }

    [XmlElement("Annotation")] public Value<string> Annotation { get; set; }

    [XmlElement("Color")] public Value<string> Color { get; set; }

    [XmlElement("LaunchMode")] public Value<int> LaunchMode { get; set; }

    [XmlElement("LaunchQuantisation")] public Value<int> LaunchQuantisation { get; set; }

    [XmlElement("TimeSignature")] public TimeSignature TimeSignature { get; set; }

    [XmlElement("Envelopes")] public Envelopes Envelopes { get; set; }

    [XmlElement("ScrollerTimePreserver")] public ScrollerTimePreserver ScrollerTimePreserver { get; set; }

    [XmlElement("TimeSelection")] public TimeSelection TimeSelection { get; set; }

    [XmlElement("Legato")] public Value<bool> Legato { get; set; }

    [XmlElement("Ram")] public Value<bool> Ram { get; set; }

    [XmlElement("GrooveSettings")] public GrooveSettings GrooveSettings { get; set; }

    [XmlElement("Disabled")] public Value<bool> Disabled { get; set; }

    [XmlElement("VelocityAmount")] public Value<decimal> VelocityAmount { get; set; }

    [XmlElement("FollowAction")] public FollowAction FollowAction { get; set; }

    [XmlElement("Grid")] public Grid Grid { get; set; }

    [XmlElement("FreezeStart")] public Value<decimal> FreezeStart { get; set; }

    [XmlElement("FreezeEnd")] public Value<decimal> FreezeEnd { get; set; }

    [XmlElement("IsWarped")] public Value<bool> IsWarped { get; set; }

    [XmlElement("TakeId")] public Value<int> TakeId { get; set; }
}

public class MidiClip : Clip
{
    [XmlArray("KeyTracks")]
    [XmlArrayItem("KeyTrack")]
    public List<KeyTrack> KeyTracks { get; set; }

    [XmlElement("PerNoteEventStore")] public PerNoteEventStore PerNoteEventStore { get; set; }

    [XmlArray("NoteProbabilityGroups")]
    [XmlArrayItem("NoteProbabilityGroup")]
    public List<NoteProbabilityGroup> NoteProbabilityGroups { get; set; }

    [XmlElement("ProbabilityGroupIdGenerator")]
    public ProbabilityGroupIdGenerator ProbabilityGroupIdGenerator { get; set; }

    [XmlElement("NoteIdGenerator")] public NoteIdGenerator NoteIdGenerator { get; set; }

    [XmlElement("BankSelectCoarse")] public Value<int> BankSelectCoarse { get; set; }

    [XmlElement("BankSelectFine")] public Value<int> BankSelectFine { get; set; }

    [XmlElement("ProgramChange")] public Value<int> ProgramChange { get; set; }

    [XmlElement("NoteEditorFoldInZoom")] public Value<int> NoteEditorFoldInZoom { get; set; }

    [XmlElement("NoteEditorFoldInScroll")] public Value<int> NoteEditorFoldInScroll { get; set; }

    [XmlElement("NoteEditorFoldOutZoom")] public Value<int> NoteEditorFoldOutZoom { get; set; }

    [XmlElement("NoteEditorFoldOutScroll")]
    public Value<int> NoteEditorFoldOutScroll { get; set; }

    [XmlElement("NoteEditorFoldScaleZoom")]
    public int NoteEditorFoldScaleZoom { get; set; }

    [XmlElement("NoteEditorFoldScaleScroll")]
    public int NoteEditorFoldScaleScroll { get; set; }

    [XmlElement("ScaleInformation")] public ScaleInformation ScaleInformation { get; set; }
    [XmlElement("IsInKey")] public bool IsInKey { get; set; }
    [XmlElement("NoteSpellingPreference")] public int NoteSpellingPreference { get; set; }

    [XmlElement("AccidentalSpellingPreference")]
    public int AccidentalSpellingPreference { get; set; }

    [XmlElement("PreferFlatRootNote")] public bool PreferFlatRootNote { get; set; }
    [XmlElement("ExpressionGrid")] public ExpressionGrid ExpressionGrid { get; set; }
}

public class AudioClip : Clip
{
    [XmlElement("SampleRef")] public SampleRef SampleRef { get; set; }

    [XmlElement("Onsets")] public Onsets Onsets { get; set; }

    [XmlElement("WarpMode")] public Value<int> WarpMode { get; set; }

    [XmlElement("GranularityTones")] public Value<int> GranularityTones { get; set; }

    [XmlElement("GranularityTexture")] public Value<int> GranularityTexture { get; set; }

    [XmlElement("FluctuationTexture")] public Value<int> FluctuationTexture { get; set; }

    [XmlElement("TransientResolution")] public Value<int> TransientResolution { get; set; }

    [XmlElement("TransientLoopMode")] public Value<int> TransientLoopMode { get; set; }

    [XmlElement("TransientEnvelope")] public Value<int> TransientEnvelope { get; set; }

    [XmlElement("ComplexProFormants")] public Value<int> ComplexProFormants { get; set; }

    [XmlElement("ComplexProEnvelope")] public Value<int> ComplexProEnvelope { get; set; }

    [XmlElement("Sync")] public Value<bool> Sync { get; set; }

    [XmlElement("HiQ")] public Value<bool> HiQ { get; set; }

    [XmlElement("Fade")] public Value<bool> Fade { get; set; }

    [XmlElement("Fades")] public Fades Fades { get; set; }

    [XmlElement("PitchCoarse")] public Value<int> PitchCoarse { get; set; }

    [XmlElement("PitchFine")] public Value<int> PitchFine { get; set; }

    [XmlElement("SampleVolume")] public Value<decimal> SampleVolume { get; set; }

    [XmlArray("WarpMarkers")]
    [XmlArrayItem("WarpMarker")]
    public List<WarpMarker> WarpMarkers { get; set; }

    [XmlElement("SavedWarpMarkersForStretched")]
    public SavedWarpMarkers SavedWarpMarkersForStretched { get; set; }

    [XmlElement("MarkersGenerated")] public Value<bool> MarkersGenerated { get; set; }

    [XmlElement("IsSongTempoLeader")] public Value<bool> IsSongTempoLeader { get; set; }
}

public class KeyTrack
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlArray("Notes")]
    [XmlArrayItem("MidiNoteEvent")]
    public List<MidiNoteEvent> Notes { get; set; }

    [XmlElement("MidiKey")] public Value<int> MidiKey { get; set; }
}

public class MidiNoteEvent
{
    [XmlAttribute("Time")] public decimal Time { get; set; }

    [XmlAttribute("Duration")] public decimal Duration { get; set; }

    [XmlAttribute("Velocity")] public int Velocity { get; set; }

    [XmlAttribute("VelocityDeviation")] public int VelocityDeviation { get; set; }

    [XmlAttribute("OffVelocity")] public int OffVelocity { get; set; }

    [XmlAttribute("Probability")] public decimal Probability { get; set; }

    [XmlAttribute("IsEnabled")] public bool IsEnabled { get; set; }

    [XmlAttribute("NoteId")] public int NoteId { get; set; }
}

public class AutomationEvent
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlAttribute("Time")] public decimal Time { get; set; }

    [XmlAttribute("Value")] public string Value { get; set; }
}

[XmlInclude(typeof(FloatEvent))]
[XmlInclude(typeof(EnumEvent))]
[XmlInclude(typeof(BoolEvent))]
public abstract class AutomationEventBase
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlAttribute("Time")] public decimal Time { get; set; }
}

public class FloatEvent : AutomationEventBase
{
    [XmlAttribute("Value")] public decimal Value { get; set; }
}

public class EnumEvent : AutomationEventBase
{
    [XmlAttribute("Value")] public int Value { get; set; }
}

public class BoolEvent : AutomationEventBase
{
    [XmlAttribute("Value")] public bool Value { get; set; }
}

public class EnvelopeTarget
{
    [XmlElement("PointeeId")] public Value<string> PointeeId { get; set; }
}

public class Automation
{
    [XmlArray("Events")]
    [XmlArrayItem("FloatEvent", typeof(FloatEvent))]
    [XmlArrayItem("EnumEvent", typeof(EnumEvent))]
    [XmlArrayItem("BoolEvent", typeof(BoolEvent))]
    public List<AutomationEventBase> Events { get; set; }

    [XmlElement("AutomationTransformViewState")]
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}

public class ScrollerTimePreserver
{
    [XmlElement("LeftTime")] public Value<decimal> LeftTime { get; set; }

    [XmlElement("RightTime")] public Value<decimal> RightTime { get; set; }
}

public class TimeSelection
{
    [XmlElement("AnchorTime")] public Value<decimal> AnchorTime { get; set; }

    [XmlElement("OtherTime")] public Value<decimal> OtherTime { get; set; }
}

public class GrooveSettings
{
    [XmlElement("GrooveId")] public Value<int> GrooveId { get; set; }
}

public class Loop
{
    [XmlElement("LoopStart")] public Value<decimal> LoopStart { get; set; }

    [XmlElement("LoopEnd")] public Value<decimal> LoopEnd { get; set; }

    [XmlElement("StartRelative")] public Value<decimal> StartRelative { get; set; }

    [XmlElement("LoopOn")] public Value<bool> LoopOn { get; set; }

    [XmlElement("OutMarker")] public Value<decimal> OutMarker { get; set; }

    [XmlElement("HiddenLoopStart")] public Value<decimal> HiddenLoopStart { get; set; }

    [XmlElement("HiddenLoopEnd")] public Value<decimal> HiddenLoopEnd { get; set; }
}

public class TimeSignature
{
    [XmlArray("TimeSignatures")]
    [XmlArrayItem("RemoteableTimeSignature")]
    public List<RemoteableTimeSignature> TimeSignatures { get; set; }
}

public class RemoteableTimeSignature
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("Numerator")] public Value<int> Numerator { get; set; }

    [XmlElement("Denominator")] public Value<int> Denominator { get; set; }

    [XmlElement("Time")] public Value<decimal> Time { get; set; }
}

public class Value<T>
{
    [XmlAttribute("Value")] public T Val { get; set; }

    public static implicit operator T(Value<T> value) => value.Val;
    public static implicit operator Value<T>(T value) => new Value<T> { Val = value };
}

public class Mixer
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")] public Value<int> LomIdView { get; set; }

    [XmlElement("IsExpanded")] public Value<bool> IsExpanded { get; set; }

    [XmlElement("On")] public On On { get; set; }

    [XmlElement("ModulationSourceCount")] public Value<int> ModulationSourceCount { get; set; }

    [XmlElement("ParametersListWrapper")] public ParametersListWrapper ParametersListWrapper { get; set; }

    [XmlElement("Pointee")] public Pointee Pointee { get; set; }

    [XmlElement("LastSelectedTimeableIndex")]
    public Value<int> LastSelectedTimeableIndex { get; set; }

    [XmlElement("LastSelectedClipEnvelopeIndex")]
    public Value<int> LastSelectedClipEnvelopeIndex { get; set; }

    [XmlElement("LastPresetRef")] public LastPresetRef LastPresetRef { get; set; }

    [XmlElement("LockedScripts")] public LockedScripts LockedScripts { get; set; }

    [XmlElement("IsFolded")] public Value<bool> IsFolded { get; set; }

    [XmlElement("ShouldShowPresetName")] public Value<bool> ShouldShowPresetName { get; set; }

    [XmlElement("UserName")] public Value<string> UserName { get; set; }

    [XmlElement("Annotation")] public Value<string> Annotation { get; set; }

    [XmlElement("SourceContext")] public Value<string> SourceContext { get; set; }

    [XmlArray("Sends")]
    [XmlArrayItem("Send")]
    public List<Send> Sends { get; set; }

    [XmlElement("Speaker")] public Speaker Speaker { get; set; }

    [XmlElement("SoloSink")] public Value<bool> SoloSink { get; set; }

    [XmlElement("PanMode")] public Value<int> PanMode { get; set; }

    [XmlElement("Pan")] public Pan Pan { get; set; }

    [XmlElement("Volume")] public Volume Volume { get; set; }

    [XmlElement("ViewStateSesstionTrackWidth")]
    public Value<decimal> ViewStateSesstionTrackWidth { get; set; }

    [XmlElement("CrossFadeState")] public CrossFadeState CrossFadeState { get; set; }

    [XmlElement("SendsListWrapper")] public SendsListWrapper SendsListWrapper { get; set; }
}

public enum MonitoringEnum
{
    [XmlEnum("0")] Off = 0,
    [XmlEnum("1")] In = 1,
    [XmlEnum("2")] Auto = 2
}

public class MainSequencer
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("On")] public On On { get; set; }

    [XmlElement("ModulationSourceCount")] public Value<int> ModulationSourceCount { get; set; }

    [XmlArray("ClipSlotList")]
    [XmlArrayItem("ClipSlot")]
    public List<ClipSlot> ClipSlotList { get; set; }

    [XmlElement("MonitoringEnum")] public Value<MonitoringEnum> MonitoringEnum { get; set; }

    [XmlElement("Sample")] public Sample Sample { get; set; }

    [XmlArray("MidiControllers")]
    [XmlArrayItem("ControllerTargets")]
    public List<MidiController> MidiControllers { get; set; }

    [XmlElement("ClipTimeable")] public ClipTimeable ClipTimeable { get; set; }

    [XmlElement("Recorder")] public Recorder Recorder { get; set; }
}

public class Scene
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("FollowAction")] public FollowAction FollowAction { get; set; }

    [XmlElement("Name")] public Value<string> Name { get; set; }

    [XmlElement("Annotation")] public Value<string> Annotation { get; set; }

    [XmlElement("Color")] public Value<string> Color { get; set; }

    [XmlElement("Tempo")] public Value<decimal> Tempo { get; set; }

    [XmlElement("IsTempoEnabled")] public Value<bool> IsTempoEnabled { get; set; }

    [XmlElement("TimeSignatureId")] public Value<string> TimeSignatureId { get; set; }

    [XmlElement("IsTimeSignatureEnabled")] public Value<bool> IsTimeSignatureEnabled { get; set; }

    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("ClipSlotsListWrapper")] public ClipSlotsListWrapper ClipSlotsListWrapper { get; set; }
}

public class Transport
{
    [XmlElement("PhaseNudgeTempo")] public Value<decimal> PhaseNudgeTempo { get; set; }

    [XmlElement("LoopOn")] public Value<bool> LoopOn { get; set; }

    [XmlElement("LoopStart")] public Value<decimal> LoopStart { get; set; }

    [XmlElement("LoopLength")] public Value<decimal> LoopLength { get; set; }

    [XmlElement("LoopIsSongStart")] public Value<bool> LoopIsSongStart { get; set; }

    [XmlElement("CurrentTime")] public Value<decimal> CurrentTime { get; set; }

    [XmlElement("PunchIn")] public Value<bool> PunchIn { get; set; }

    [XmlElement("PunchOut")] public Value<bool> PunchOut { get; set; }

    [XmlElement("MetronomeTickDuration")] public Value<decimal> MetronomeTickDuration { get; set; }

    [XmlElement("DrawMode")] public Value<bool> DrawMode { get; set; }
}

public class ViewStates
{
    [XmlElement("MixerInArrangement")] public Value<int> MixerInArrangement { get; set; }
    [XmlElement("ArrangerMixerIO")] public Value<int> ArrangerMixerIO { get; set; }
    [XmlElement("ArrangerMixerSends")] public Value<int> ArrangerMixerSends { get; set; }
    [XmlElement("ArrangerMixerReturns")] public Value<int> ArrangerMixerReturns { get; set; }
    [XmlElement("ArrangerMixerVolume")] public Value<int> ArrangerMixerVolume { get; set; }

    [XmlElement("ArrangerMixerTrackOptions")]
    public Value<int> ArrangerMixerTrackOptions { get; set; }

    [XmlElement("ArrangerMixerCrossFade")] public Value<int> ArrangerMixerCrossFade { get; set; }

    [XmlElement("ArrangerMixerTrackPerformanceImpactMeter")]
    public Value<int> ArrangerMixerTrackPerformanceImpactMeter { get; set; }

    [XmlElement("MixerInSession")] public Value<int> MixerInSession { get; set; }
    [XmlElement("SessionIO")] public Value<int> SessionIO { get; set; }
    [XmlElement("SessionSends")] public Value<int> SessionSends { get; set; }
    [XmlElement("SessionReturns")] public Value<int> SessionReturns { get; set; }
    [XmlElement("SessionVolume")] public Value<int> SessionVolume { get; set; }
    [XmlElement("SessionTrackOptions")] public Value<int> SessionTrackOptions { get; set; }
    [XmlElement("SessionCrossFade")] public Value<int> SessionCrossFade { get; set; }
    [XmlElement("SessionTrackDelay")] public Value<int> SessionTrackDelay { get; set; }

    [XmlElement("SessionTrackPerformanceImpactMeter")]
    public Value<int> SessionTrackPerformanceImpactMeter { get; set; }

    [XmlElement("SessionShowOverView")] public Value<int> SessionShowOverView { get; set; }
    [XmlElement("ArrangerIO")] public Value<int> ArrangerIO { get; set; }
    [XmlElement("ArrangerReturns")] public Value<int> ArrangerReturns { get; set; }
    [XmlElement("ArrangerVolume")] public Value<int> ArrangerVolume { get; set; }
    [XmlElement("ArrangerTrackOptions")] public Value<int> ArrangerTrackOptions { get; set; }
    [XmlElement("ArrangerShowOverView")] public Value<int> ArrangerShowOverView { get; set; }
    [XmlElement("ArrangerTrackDelay")] public Value<int> ArrangerTrackDelay { get; set; }
    [XmlElement("ArrangerMixer")] public Value<int> ArrangerMixer { get; set; }
}

public class TrackDelay
{
    [XmlElement("Value")] public Value<decimal> Value { get; set; }

    [XmlElement("IsValueSampleBased")] public Value<bool> IsValueSampleBased { get; set; }
}

public class On
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("Manual")] public Value<bool> Manual { get; set; }

    [XmlElement("AutomationTarget")] public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("MidiCCOnOffThresholds")] public MidiCCOnOffThresholds MidiCCOnOffThresholds { get; set; }
}

public class AutomationTarget
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("LockEnvelope")] public Value<int> LockEnvelope { get; set; }
}

public class MidiCCOnOffThresholds
{
    [XmlElement("Min")] public Value<int> Min { get; set; }

    [XmlElement("Max")] public Value<int> Max { get; set; }
}

public class Routing
{
    [XmlElement("Target")] public Value<string> Target { get; set; }

    [XmlElement("UpperDisplayString")] public Value<string> UpperDisplayString { get; set; }

    [XmlElement("LowerDisplayString")] public Value<string> LowerDisplayString { get; set; }

    [XmlElement("MpeSettings")] public MpeSettings MpeSettings { get; set; }
}

public class AudioInputRouting : Routing
{
}

public class AudioOutputRouting : Routing
{
}

public class MidiInputRouting : Routing
{
}

public class MidiOutputRouting : Routing
{
}

public class MpeSettings
{
    [XmlElement("ZoneType")] public Value<int> ZoneType { get; set; }

    [XmlElement("FirstNoteChannel")] public Value<int> FirstNoteChannel { get; set; }

    [XmlElement("LastNoteChannel")] public Value<int> LastNoteChannel { get; set; }
}

public class ClipSlot
{
    [XmlAttribute("Id")] public string Id { get; set; }

    public Value<int> LomId { get; set; }

    [XmlElement("ClipSlot")] public ClipSlotValue ClipData { get; set; }

    public Value<bool> HasStop { get; set; }

    public Value<bool> NeedRefreeze { get; set; }
}

public class ClipSlotValue
{
    [XmlElement("Value")] public string Value { get; set; }

    [XmlElement("Clip")] public Clip Clip { get; set; }
}

public class Recorder
{
    [XmlElement("IsArmed")] public Value<bool> IsArmed { get; set; }

    [XmlElement("TakeCounter")] public Value<int> TakeCounter { get; set; }
}

public class Volume
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("Manual")] public Value<decimal> Manual { get; set; }

    [XmlElement("MidiControllerRange")] public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")] public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")] public ModulationTarget ModulationTarget { get; set; }
}

public class MidiControllerRange
{
    [XmlElement("Min")] public Value<decimal> Min { get; set; }

    [XmlElement("Max")] public Value<decimal> Max { get; set; }
}

public class ModulationTarget
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("LockEnvelope")] public Value<int> LockEnvelope { get; set; }
}

public class Pan
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("Manual")] public Value<decimal> Manual { get; set; }

    [XmlElement("MidiControllerRange")] public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")] public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")] public ModulationTarget ModulationTarget { get; set; }
}

public class Send
{
    [XmlElement("LomId")] public Value<int> LomId { get; set; }

    [XmlElement("Manual")] public Value<decimal> Manual { get; set; }

    [XmlElement("MidiControllerRange")] public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")] public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")] public ModulationTarget ModulationTarget { get; set; }
}

public class Fades
{
    [XmlElement("FadeInLength")] public Value<decimal> FadeInLength { get; set; }

    [XmlElement("FadeOutLength")] public Value<decimal> FadeOutLength { get; set; }

    [XmlElement("ClipFadesAreInitialized")]
    public Value<bool> ClipFadesAreInitialized { get; set; }

    [XmlElement("CrossfadeInState")] public Value<int> CrossfadeInState { get; set; }

    [XmlElement("FadeInCurveSkew")] public Value<decimal> FadeInCurveSkew { get; set; }

    [XmlElement("FadeInCurveSlope")] public Value<decimal> FadeInCurveSlope { get; set; }

    [XmlElement("FadeOutCurveSkew")] public Value<decimal> FadeOutCurveSkew { get; set; }

    [XmlElement("FadeOutCurveSlope")] public Value<decimal> FadeOutCurveSlope { get; set; }

    [XmlElement("IsDefaultFadeIn")] public Value<bool> IsDefaultFadeIn { get; set; }

    [XmlElement("IsDefaultFadeOut")] public Value<bool> IsDefaultFadeOut { get; set; }
}

public class SampleRef
{
    [XmlElement("FileRef")] public FileRef FileRef { get; set; }

    [XmlElement("LastModDate")] public Value<long> LastModDate { get; set; }

    [XmlElement("SourceContext")] public SourceContext SourceContext { get; set; }

    [XmlElement("SampleUsageHint")] public Value<int> SampleUsageHint { get; set; }

    [XmlElement("DefaultDuration")] public Value<long> DefaultDuration { get; set; }

    [XmlElement("DefaultSampleRate")] public Value<int> DefaultSampleRate { get; set; }
}

public class FileRef
{
    [XmlElement("RelativePathType")] public Value<int> RelativePathType { get; set; }

    [XmlElement("RelativePath")] public Value<string> RelativePath { get; set; }

    [XmlElement("Path")] public Value<string> Path { get; set; }

    [XmlElement("Type")] public Value<int> Type { get; set; }

    [XmlElement("LivePackName")] public Value<string> LivePackName { get; set; }

    [XmlElement("LivePackId")] public Value<string> LivePackId { get; set; }

    [XmlElement("OriginalFileSize")] public Value<long> OriginalFileSize { get; set; }

    [XmlElement("OriginalCrc")] public Value<int> OriginalCrc { get; set; }
}

public class WarpMarker
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("SecTime")] public Value<decimal> SecTime { get; set; }

    [XmlElement("BeatTime")] public Value<decimal> BeatTime { get; set; }
}

public class SendPreBool
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlAttribute("Value")] public bool Value { get; set; }
}

public class SourceContext
{
    [XmlElement("Value")] public string Value { get; set; }
}

public class LastPresetRef
{
    [XmlElement("Value")] public string Value { get; set; }
}

public class LockedScripts
{
    // Empty in examples but included for XML serialization
}

public class ExpressionLane
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("Type")] public Value<int> Type { get; set; }

    [XmlElement("Size")] public Value<decimal> Size { get; set; }

    [XmlElement("IsMinimized")] public Value<bool> IsMinimized { get; set; }
}

public class TrackName
{
    [XmlElement("EffectiveName")] public Value<string> EffectiveName { get; set; }
    [XmlElement("UserName")] public Value<string> UserName { get; set; }
    [XmlElement("Annotation")] public Value<string> Annotation { get; set; }
    [XmlElement("MemorizedFirstClipName")] public Value<string> MemorizedFirstClipName { get; set; }
}

public class AutomationLanes
{
    [XmlArray("AutomationLanes")]
    [XmlArrayItem("AutomationLane")]
    public List<AutomationLane> Lanes { get; set; }

    [XmlElement("AreAdditionalAutomationLanesFolded")]
    public Value<bool> AreAdditionalAutomationLanesFolded { get; set; }
}

public class AutomationLane
{
    public Value<string> Id { get; set; }
    public Value<int> SelectedDevice { get; set; }
    public Value<int> SelectedEnvelope { get; set; }
    public Value<bool> IsContentSelectedInDocument { get; set; }
    public Value<decimal> LaneHeight { get; set; }
}

public class ClipEnvelopeChooserViewState
{
    public Value<int> SelectedDevice { get; set; }
    public Value<int> SelectedEnvelope { get; set; }
    public Value<bool> PreferModulationVisible { get; set; }
}

public class FreezeSequencer
{
    public Value<int> LomId { get; set; }
    public Value<int> LomIdView { get; set; }
    public Value<bool> IsExpanded { get; set; }
    public On On { get; set; }
    public Value<int> ModulationSourceCount { get; set; }
    public ParametersListWrapper ParametersListWrapper { get; set; }
    public Pointee Pointee { get; set; }
    public Value<int> LastSelectedTimeableIndex { get; set; }
    public Value<int> LastSelectedClipEnvelopeIndex { get; set; }
    public LastPresetRef LastPresetRef { get; set; }
    public LockedScripts LockedScripts { get; set; }
    public Value<bool> IsFolded { get; set; }
    public Value<bool> ShouldShowPresetName { get; set; }
    public Value<string> UserName { get; set; }
    public Value<string> Annotation { get; set; }
    public SourceContext SourceContext { get; set; }
    public List<ClipSlot> ClipSlotList { get; set; }
    public Value<MonitoringEnum> MonitoringEnum { get; set; }
    public Value<bool> KeepRecordMonitoringLatency { get; set; }
    public Sample Sample { get; set; }
    public VolumeModulationTarget VolumeModulationTarget { get; set; }
    public ModulationTarget TranspositionModulationTarget { get; set; }
    public ModulationTarget TransientEnvelopeModulationTarget { get; set; }
    public ModulationTarget GrainSizeModulationTarget { get; set; }
    public ModulationTarget FluxModulationTarget { get; set; }
    public ModulationTarget SampleOffsetModulationTarget { get; set; }
    public ModulationTarget ComplexProFormantsModulationTarget { get; set; }
    public ModulationTarget ComplexProEnvelopeModulationTarget { get; set; }
    public Value<decimal> PitchViewScrollPosition { get; set; }
    public Value<decimal> SampleOffsetModulationScrollPosition { get; set; }
    public Recorder Recorder { get; set; }
}

public class PerNoteEventStore
{
    public List<EventList> EventLists { get; set; }
}

public class ProbabilityGroupIdGenerator
{
    public Value<int> NextId { get; set; }
}

public class NoteIdGenerator
{
    public Value<int> NextId { get; set; }
}

public class Envelopes
{
    [XmlElement("Envelopes")] public List<ClipEnvelope> EnvelopeCollection { get; set; }
}

public class ClipEnvelope
{
    public Value<string> Id { get; set; }
    public EnvelopeTarget EnvelopeTarget { get; set; }
    public Automation Automation { get; set; }
    public LoopSlot LoopSlot { get; set; }
    public ScrollerTimePreserver ScrollerTimePreserver { get; set; }
}

public class FollowAction
{
    public Value<decimal> FollowTime { get; set; }
    public Value<bool> IsLinked { get; set; }
    public Value<int> LoopIterations { get; set; }
    public Value<int> FollowActionA { get; set; }
    public Value<int> FollowActionB { get; set; }
    public Value<int> FollowChanceA { get; set; }
    public Value<int> FollowChanceB { get; set; }
    public Value<int> JumpIndexA { get; set; }
    public Value<int> JumpIndexB { get; set; }
    public Value<bool> FollowActionEnabled { get; set; }
}

public class Grid
{
    public Value<int> FixedNumerator { get; set; }
    public Value<int> FixedDenominator { get; set; }
    public Value<decimal> GridIntervalPixel { get; set; }
    public Value<int> Ntoles { get; set; }
    public Value<bool> SnapToGrid { get; set; }
    public Value<bool> Fixed { get; set; }
}

public class ScaleInformation
{
    public Value<int> RootNote { get; set; }
    public Value<string> Name { get; set; }
}

public class ExpressionGrid
{
    public Value<int> FixedNumerator { get; set; }
    public Value<int> FixedDenominator { get; set; }
    public Value<decimal> GridIntervalPixel { get; set; }
    public Value<int> Ntoles { get; set; }
    public Value<bool> SnapToGrid { get; set; }
    public Value<bool> Fixed { get; set; }
}

public class Onsets
{
    public List<UserOnset> UserOnsets { get; set; }
    public Value<bool> HasUserOnsets { get; set; }
}

public class SavedWarpMarkers
{
    // Usually empty in the examples, but included for completeness
}

public class ViewData
{
    // In examples this is usually serialized as "{}"
    public string Value { get; set; }
}

public class TakeLanes
{
    [XmlElement("TakeLanes")] public List<TakeLane> LaneCollection { get; set; }
    public Value<bool> AreTakeLanesFolded { get; set; }
}

public class ArrangerAutomation
{
    public List<AutomationEvent> Events { get; set; }
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}

public class TakeLane
{
    public Value<string> Id { get; set; }
    public Value<decimal> Height { get; set; }
    public Value<bool> IsContentSelectedInDocument { get; set; }
    public Value<string> Name { get; set; }
    public Value<string> Annotation { get; set; }
    public ClipAutomation ClipAutomation { get; set; }
}

public class ClipAutomation
{
    public List<MidiClip> Events { get; set; }
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}

[XmlType("Envelope")]
public class Envelope
{
    [XmlAttribute("Id")] public string Id { get; set; }

    [XmlElement("EnvelopeTarget")] public EnvelopeTarget EnvelopeTarget { get; set; }

    [XmlElement("Automation")] public Automation Automation { get; set; }
}

[XmlRoot("AutomationEnvelopes")]
public class AutomationEnvelopes
{
    [XmlArray("Envelopes")]
    [XmlArrayItem("AutomationEnvelope")]
    public List<Envelope> Envelopes { get; set; }
}

public class DevicesListWrapper
{
    [XmlAttribute("LomId")] public int LomId { get; set; }
}

public class ClipSlotsListWrapper
{
    [XmlAttribute("LomId")] public int LomId { get; set; }
}

public class SendsListWrapper
{
    [XmlAttribute("LomId")] public int LomId { get; set; }
}

public class ControllerLayoutCustomization
{
    public Value<int> PitchClassSource { get; set; }
    public Value<int> OctaveSource { get; set; }
    public Value<int> KeyNoteTarget { get; set; }
    public Value<int> StepSize { get; set; }
    public Value<int> OctaveEvery { get; set; }
    public Value<int> AllowedKeys { get; set; }
    public Value<int> FillerKeysMapTo { get; set; }
}

public class Devices
{
    public List<Device> DeviceList { get; set; }
}

public class SignalModulations
{
    // Empty in examples but included for completeness
}

public class ParametersListWrapper
{
    [XmlAttribute("LomId")] public int LomId { get; set; }
}

public class Pointee
{
    public Value<string> Id { get; set; }
}

public class Speaker
{
    public Value<int> LomId { get; set; }
    public Value<bool> Manual { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public MidiCCOnOffThresholds MidiCCOnOffThresholds { get; set; }
}

public class SplitStereoPan
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}

public class CrossFadeState
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}

public class TempoAutomation
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}

public class GlobalGrooveAmount
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}

public class CrossFade
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}

public class Sample
{
    public ArrangerAutomation ArrangerAutomation { get; set; }
}

public class MidiController
{
    public Value<string> Id { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}

public class ClipTimeable
{
    public ArrangerAutomation ArrangerAutomation { get; set; }
}

public class VolumeModulationTarget
{
    public Value<int> LockEnvelope { get; set; }
}

public class EventList
{
    // Empty in examples but included for completeness
}

public class LoopSlot
{
    public string Value { get; set; }
}

public class AutomationTransformViewState
{
    public Value<bool> IsTransformPending { get; set; }
    public TimeAndValueTransforms TimeAndValueTransforms { get; set; }
}

public class TimeAndValueTransforms
{
    // Empty in examples but included for completeness
}

public class UserOnset
{
    // Empty in examples but included for completeness
}

public class SendsPre
{
    public Value<bool> Value { get; set; }
}

public class Locator
{
    public Value<string> Time { get; set; }
    public Value<string> Name { get; set; }
    public Value<string> Annotation { get; set; }
}

public class ContentLane
{
    public Value<int> Type { get; set; }
    public Value<decimal> Size { get; set; }
    public Value<bool> IsMinimized { get; set; }
}

public class NoteProbabilityGroup
{
    // Empty in examples but included for completeness
}

public class Device
{
    public Value<string> Id { get; set; }
    public Value<int> LomId { get; set; }
    public Value<bool> IsExpanded { get; set; }
    public On On { get; set; }
    public Value<int> ModulationSourceCount { get; set; }
    public ParametersListWrapper ParametersListWrapper { get; set; }
    public Value<string> LastPresetRef { get; set; }
    public Value<bool> IsFolded { get; set; }
    public Value<bool> ShouldShowPresetName { get; set; }
    public Value<string> UserName { get; set; }
    public Value<string> Annotation { get; set; }
    public List<Parameter> Parameters { get; set; }
}

public class Parameter
{
    public Value<string> Id { get; set; }
    public Value<decimal> Value { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}

public class SequencerNavigator
{
    public BeatTimeHelper BeatTimeHelper { get; set; }
    public ScrollerPos ScrollerPos { get; set; }
    public ClientSize ClientSize { get; set; }
}

public class BeatTimeHelper
{
    public Value<decimal> CurrentZoom { get; set; }
}

public class ScrollerPos
{
    public Value<int> X { get; set; }
    public Value<int> Y { get; set; }
}

public class ClientSize
{
    public Value<int> X { get; set; }
    public Value<int> Y { get; set; }
}