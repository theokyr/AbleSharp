using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlInclude(typeof(MidiTrack))]
[XmlInclude(typeof(AudioTrack))] 
[XmlInclude(typeof(ReturnTrack))]
[XmlInclude(typeof(MainTrack))]
[XmlInclude(typeof(PreHearTrack))]
[XmlInclude(typeof(GroupTrack))]
public abstract class Track
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; }

    [XmlElement("IsContentSelectedInDocument")]
    public Value<bool> IsContentSelectedInDocument { get; set; }

    [XmlElement("PreferredContentViewMode")]
    public Value<int> PreferredContentViewMode { get; set; }

    [XmlElement("TrackDelay")]
    public TrackDelay TrackDelay { get; set; }

    [XmlElement("Name")]
    public TrackName Name { get; set; }

    [XmlElement("Color")]
    public Value<int> Color { get; set; }

    [XmlElement("AutomationEnvelopes")]
    public AutomationEnvelopes AutomationEnvelopes { get; set; }

    [XmlElement("TrackGroupId")]
    public Value<int> TrackGroupId { get; set; }

    [XmlElement("TrackUnfolded")]
    public Value<bool> TrackUnfolded { get; set; }

    [XmlElement("DevicesListWrapper")]
    public DevicesListWrapper DevicesListWrapper { get; set; }

    [XmlElement("ClipSlotsListWrapper")]
    public ClipSlotsListWrapper ClipSlotsListWrapper { get; set; }

    [XmlElement("ViewData")]
    public Value<string> ViewData { get; set; }

    [XmlElement("TakeLanes")] 
    public TakeLanes TakeLanes { get; set; } = new()
    {
        LaneCollection = new List<TakeLane>(),
        AreTakeLanesFolded = new Value<bool> { Val = true }
    };

    [XmlElement("LinkedTrackGroupId")]
    public Value<int> LinkedTrackGroupId { get; set; }

    [XmlElement("DeviceChain")]
    public DeviceChain DeviceChain { get; set; }

    [XmlElement("ReWireDeviceMidiTargetId")]
    public Value<int> ReWireDeviceMidiTargetId { get; set; }

    [XmlElement("SavedPlayingSlot")]
    public Value<int> SavedPlayingSlot { get; set; }

    [XmlElement("SavedPlayingOffset")]
    public Value<decimal> SavedPlayingOffset { get; set; }

    [XmlElement("VelocityDetail")]
    public Value<int> VelocityDetail { get; set; }
    
    public Track()
    {
        // Initialize default values
        LomId = new Value<int> { Val = 0 };
        LomIdView = new Value<int> { Val = 0 };
        IsContentSelectedInDocument = new Value<bool> { Val = false };
        PreferredContentViewMode = new Value<int> { Val = 0 };
        TrackDelay = new TrackDelay
        {
            Value = new Value<decimal> { Val = 0 },
            IsValueSampleBased = new Value<bool> { Val = false }
        };
        Name = new TrackName
        {
            EffectiveName = new Value<string> { Val = "" },
            UserName = new Value<string> { Val = "" },
            Annotation = new Value<string> { Val = "" },
            MemorizedFirstClipName = new Value<string> { Val = "" }
        };
        Color = new Value<int> { Val = -1 };
        AutomationEnvelopes = new AutomationEnvelopes { Envelopes = new List<AutomationEnvelope>() };
        TrackGroupId = new Value<int> { Val = -1 };
        TrackUnfolded = new Value<bool> { Val = false };
        DevicesListWrapper = new DevicesListWrapper { LomId = 0 };
        ClipSlotsListWrapper = new ClipSlotsListWrapper { LomId = 0 };
        ViewData = new Value<string> { Val = "{}" };
        LinkedTrackGroupId = new Value<int> { Val = -1 };
        DeviceChain = new DeviceChain();
    }
}