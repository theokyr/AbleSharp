using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlType("AudioTrack")]
public class AudioTrack : FreezableTrack
{
    [XmlElement("SavedPlayingSlot")]
    public Value<int> SavedPlayingSlot { get; set; }

    [XmlElement("SavedPlayingOffset")]
    public Value<decimal> SavedPlayingOffset { get; set; }

    [XmlElement("VelocityDetail")]
    public Value<int> VelocityDetail { get; set; }
}