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
    [XmlAttribute("Id")]
    public string Id { get; set; }

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
    public TakeLanes TakeLanes { get; set; }

    [XmlElement("LinkedTrackGroupId")]
    public Value<int> LinkedTrackGroupId { get; set; }

    [XmlElement("DeviceChain")]
    public DeviceChain DeviceChain { get; set; }

    [XmlElement("ReWireDeviceMidiTargetId")]
    public Value<int> ReWireDeviceMidiTargetId { get; set; }
}