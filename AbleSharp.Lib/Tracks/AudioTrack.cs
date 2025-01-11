using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlType("AudioTrack")]
public class AudioTrack : Track
{
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
}