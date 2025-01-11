using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class LiveSet
{
    public Value<int> NextPointeeId { get; set; }
    public Value<int> OverwriteProtectionNumber { get; set; }
    public Value<int> LomId { get; set; }
    public Value<int> LomIdView { get; set; }

    [XmlArray("Tracks")]
    [XmlArrayItem("MidiTrack", typeof(MidiTrack))]
    [XmlArrayItem("AudioTrack", typeof(AudioTrack))]
    [XmlArrayItem("GroupTrack", typeof(GroupTrack))]
    public List<Track> Tracks { get; set; }

    [XmlElement("MasterTrack")]
    public MainTrack MainTrack { get; set; }

    [XmlElement("PreHearTrack")]
    public PreHearTrack PreHearTrack { get; set; }

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

    [XmlElement("UseWarperLegacyHiQMode")]
    public Value<bool> UseWarperLegacyHiQMode { get; set; }

    [XmlElement("SignalModulations")]
    public object SignalModulationsTop { get; set; } = new object();

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
    public GroovePool GroovePool { get; set; } = new GroovePool();
}