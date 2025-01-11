using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlType("MidiTrack")]
public class MidiTrack : Track
{
    [XmlElement("ReWireSlaveMidiTargetId")]
    public Value<int> ReWireSlaveMidiTargetId { get; set; }

    [XmlElement("PitchbendRange")]
    public Value<int> PitchbendRange { get; set; }

    [XmlElement("IsTuned")]
    public Value<bool> IsTuned { get; set; }

    [XmlElement("SavedPlayingSlot")]
    public Value<int> SavedPlayingSlot { get; set; }

    [XmlElement("SavedPlayingOffset")]
    public Value<decimal> SavedPlayingOffset { get; set; }

    [XmlElement("Freeze")]
    public Value<bool> Freeze { get; set; }

    [XmlElement("VelocityDetail")]
    public Value<int> VelocityDetail { get; set; }

    [XmlElement("NeedArrangerRefreeze")]
    public Value<bool> NeedArrangerRefreeze { get; set; }

    [XmlElement("PostProcessFreezeClips")]
    public Value<int> PostProcessFreezeClips { get; set; }

    [XmlElement("ControllerLayoutRemoteable")]
    public Value<int> ControllerLayoutRemoteable { get; set; }

    [XmlElement("ControllerLayoutCustomization")]
    public ControllerLayoutCustomization ControllerLayoutCustomization { get; set; }
}