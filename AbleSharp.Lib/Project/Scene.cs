using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Scene
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("FollowAction")]
    public FollowAction FollowAction { get; set; } = new();

    [XmlElement("Name")]
    public Value<string> Name { get; set; } = new() { Val = "" };

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; } = new() { Val = "" };

    [XmlElement("Color")]
    public Value<int> Color { get; set; } = new() { Val = -1 };

    [XmlElement("Tempo")]
    public Value<decimal> Tempo { get; set; } = new() { Val = 120 };

    [XmlElement("IsTempoEnabled")]
    public Value<bool> IsTempoEnabled { get; set; } = new() { Val = false };

    [XmlElement("TimeSignatureId")]
    public Value<string> TimeSignatureId { get; set; } = new() { Val = "201" };

    [XmlElement("IsTimeSignatureEnabled")]
    public Value<bool> IsTimeSignatureEnabled { get; set; } = new() { Val = false };

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; } = new() { Val = 0 };

    [XmlElement("ClipSlotsListWrapper")]
    public ClipSlotsListWrapper ClipSlotsListWrapper { get; set; } = new() { LomId = 0 };
}