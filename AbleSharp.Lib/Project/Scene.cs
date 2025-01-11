using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Scene
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("FollowAction")]
    public FollowAction FollowAction { get; set; }

    [XmlElement("Name")]
    public Value<string> Name { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("Color")]
    public Value<string> Color { get; set; }

    [XmlElement("Tempo")]
    public Value<decimal> Tempo { get; set; }

    [XmlElement("IsTempoEnabled")]
    public Value<bool> IsTempoEnabled { get; set; }

    [XmlElement("TimeSignatureId")]
    public Value<string> TimeSignatureId { get; set; }

    [XmlElement("IsTimeSignatureEnabled")]
    public Value<bool> IsTimeSignatureEnabled { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("ClipSlotsListWrapper")]
    public ClipSlotsListWrapper ClipSlotsListWrapper { get; set; }
}