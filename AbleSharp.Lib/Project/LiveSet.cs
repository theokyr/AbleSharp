using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class LiveSet
{
    [XmlElement("NextPointeeId")]
    public Value<int> NextPointeeId { get; set; }

    [XmlElement("OverwriteProtectionNumber")]
    public Value<int> OverwriteProtectionNumber { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; }

    [XmlArray("Tracks")]
    [XmlArrayItem("MidiTrack", typeof(MidiTrack))]
    [XmlArrayItem("AudioTrack", typeof(AudioTrack))]
    [XmlArrayItem("GroupTrack", typeof(GroupTrack))]
    public List<FreezableTrack> Tracks { get; set; }

    [XmlElement("MainTrack")]
    public MainTrack MainTrack { get; set; }

    [XmlElement("PreHearTrack")]
    public PreHearTrack PreHearTrack { get; set; }

    [XmlArray("Scenes")]
    [XmlArrayItem("Scene")]
    public List<Scene> Scenes { get; set; }

    [XmlElement("Transport")]
    public Transport Transport { get; set; }

    [XmlElement("SendsPre")]
    public List<SendsPre> SendsPre { get; set; }

    [XmlArray("Locators")]
    [XmlArrayItem("Locator")]
    public List<Locator> Locators { get; set; }

    [XmlElement("AutomationMode")]
    public Value<bool> AutomationMode { get; set; }

    [XmlElement("SnapAutomationToGrid")]
    public Value<bool> SnapAutomationToGrid { get; set; }

    [XmlElement("ArrangementOverdub")]
    public Value<bool> ArrangementOverdub { get; set; }

    [XmlElement("GlobalQuantisation")] 
    public Value<int> GlobalQuantisation { get; set; } = new() { Val = 4 };

    [XmlElement("AutoQuantisation")]
    public Value<int> AutoQuantisation { get; set; } = new() { Val = 0 };

    [XmlElement("Grid")] 
    public Grid Grid { get; set; } = new();

    [XmlElement("ViewData")]
    public Value<string> ViewData { get; set; }

    [XmlElement("MidiFoldIn")]
    public Value<bool> MidiFoldIn { get; set; }

    [XmlElement("MidiFoldMode")]
    public Value<int> MidiFoldMode { get; set; }

    [XmlElement("MultiClipFocusMode")]
    public Value<bool> MultiClipFocusMode { get; set; }

    [XmlElement("MultiClipLoopBarHeight")]
    public Value<decimal> MultiClipLoopBarHeight { get; set; }

    [XmlElement("MidiPrelisten")]
    public Value<bool> MidiPrelisten { get; set; }

    [XmlElement("ScaleInformation")]
    public ScaleInformation ScaleInformation { get; set; }

    [XmlElement("InKey")]
    public Value<bool> InKey { get; set; }

    [XmlElement("SmpteFormat")]
    public Value<int> SmpteFormat { get; set; }

    [XmlElement("TimeSelection")]
    public TimeSelection TimeSelection { get; set; }

    [XmlElement("SequencerNavigator")]
    public SequencerNavigator SequencerNavigator { get; set; }

    [XmlElement("IsContentSplitterOpen")]
    public Value<bool> IsContentSplitterOpen { get; set; }

    [XmlElement("IsExpressionSplitterOpen")]
    public Value<bool> IsExpressionSplitterOpen { get; set; }

    [XmlElement("ExpressionLanes")]
    public List<ExpressionLane> ExpressionLanes { get; set; }

    [XmlElement("ContentLanes")] 
    public List<ContentLane> ContentLanes { get; set; }

    [XmlElement("ViewStates")]
    public ViewStates ViewStates { get; set; }

    [XmlElement("ChooserBar")]
    public Value<int> ChooserBar { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("SoloOrPflSavedValue")]
    public Value<bool> SoloOrPflSavedValue { get; set; }

    [XmlElement("SoloInPlace")]
    public Value<bool> SoloInPlace { get; set; }

    [XmlElement("CrossfadeCurve")]
    public Value<int> CrossfadeCurve { get; set; }

    [XmlElement("LatencyCompensation")]
    public Value<int> LatencyCompensation { get; set; }

    [XmlElement("HighlightedTrackIndex")]
    public Value<int> HighlightedTrackIndex { get; set; }

    [XmlElement("ColorSequenceIndex")]
    public Value<int> ColorSequenceIndex { get; set; }

    [XmlElement("AccidentalSpellingPreference")]
    public Value<int> AccidentalSpellingPreference { get; set; }

    [XmlElement("PreferFlatRootNote")]
    public Value<bool> PreferFlatRootNote { get; set; }

    [XmlElement("SignalModulations")]
    public object SignalModulationsTop { get; set; } = new();

    [XmlElement("TracksListWrapper")]
    public TracksListWrapper TracksListWrapper { get; set; }

    [XmlElement("VisibleTracksListWrapper")]
    public TracksListWrapper VisibleTracksListWrapper { get; set; }

    [XmlElement("ReturnTracksListWrapper")]
    public TracksListWrapper ReturnTracksListWrapper { get; set; }

    [XmlElement("ScenesListWrapper")]
    public TracksListWrapper ScenesListWrapper { get; set; }

    [XmlElement("CuePointsListWrapper")]
    public TracksListWrapper CuePointsListWrapper { get; set; }

    [XmlElement("GroovePool")]
    public GroovePool GroovePool { get; set; } = new();

    // TODO: temporarily disabled :)
    // [XmlElement("NoteAlgorithms")]
    // public NoteAlgorithms NoteAlgorithms { get; set; } = new();

    [XmlElement("WaveformVerticalZoomFactor")]
    public Value<int> WaveformVerticalZoomFactor { get; set; } = new() { Val = 1 };

    [XmlElement("IsWaveformVerticalZoomActive")]
    public Value<bool> IsWaveformVerticalZoomActive { get; set; } = new() { Val = true };

    [XmlElement("ViewStateMainWindowClipDetailOpen")]
    public Value<bool> ViewStateMainWindowClipDetailOpen { get; set; } = new() { Val = true };

    [XmlElement("ViewStateMainWindowDeviceDetailOpen")]
    public Value<bool> ViewStateMainWindowDeviceDetailOpen { get; set; } = new() { Val = false };

    [XmlElement("ViewStateSecondWindowClipDetailOpen")]
    public Value<bool> ViewStateSecondWindowClipDetailOpen { get; set; } = new() { Val = false };

    [XmlElement("ViewStateSecondWindowDeviceDetailOpen")]
    public Value<bool> ViewStateSecondWindowDeviceDetailOpen { get; set; } = new() { Val = true };

    [XmlElement("ViewStateMainWindowHiddenOtherDocViewTypeClipDetailOpen")]
    public Value<bool> ViewStateMainWindowHiddenOtherDocViewTypeClipDetailOpen { get; set; } = new() { Val = false };

    [XmlElement("ViewStateMainWindowHiddenOtherDocViewTypeDeviceDetailOpen")]
    public Value<bool> ViewStateMainWindowHiddenOtherDocViewTypeDeviceDetailOpen { get; set; } = new() { Val = true };

    [XmlElement("ShowVideoWindow")]
    public Value<bool> ShowVideoWindow { get; set; } = new() { Val = true };

    [XmlElement("TrackHeaderWidth")]
    public Value<int> TrackHeaderWidth { get; set; } = new() { Val = 93 };

    [XmlElement("ViewStateFxSlotCount")]
    public Value<int> ViewStateFxSlotCount { get; set; } = new() { Val = 4 };

    [XmlElement("ViewStateSessionMixerHeight")]
    public Value<int> ViewStateSessionMixerHeight { get; set; } = new() { Val = 120 };

    [XmlElement("ViewStateArrangerMixerVolumeSectionHeight")]
    public Value<int> ViewStateArrangerMixerVolumeSectionHeight { get; set; } = new() { Val = 120 };

    [XmlElement("ShouldSceneTempoAndTimeSignatureBeVisible")]
    public Value<bool> ShouldSceneTempoAndTimeSignatureBeVisible { get; set; } = new() { Val = false };

    [XmlElement("AutoColorPickerForPlayerAndGroupTracks")]
    public AutoColorPicker AutoColorPickerForPlayerAndGroupTracks { get; set; } = new();

    [XmlElement("AutoColorPickerForReturnAndMainTracks")]
    public AutoColorPicker AutoColorPickerForReturnAndMainTracks { get; set; } = new();

    [XmlElement("VideoWindowRect")]
    public VideoWindowRect VideoWindowRect { get; set; } = new();

    [XmlElement("SessionScrollPos")]
    public ScrollPosition SessionScrollPos { get; set; } = new();

    [XmlElement("LinkedTrackGroups")]
    public LinkedTrackGroups LinkedTrackGroups { get; set; } = new();

    [XmlElement("DetailClipKeyMidis")]
    public DetailClipKeyMidis DetailClipKeyMidis { get; set; } = new();

    [XmlElement("UseWarperLegacyHiQMode")]
    public Value<bool> UseWarperLegacyHiQMode { get; set; } = new() { Val = false };

    [XmlElement("TuningSystems")]
    public TuningSystems TuningSystems { get; set; } = new();
}