using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class OrnamentAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("FlamEnabled")]
    public Value<bool> FlamEnabled { get; set; } = new() { Val = true };

    [XmlElement("FlamPosition")]
    public Value<decimal> FlamPosition { get; set; } = new() { Val = -0.200000003M };

    [XmlElement("FlamVelocity")]
    public Value<decimal> FlamVelocity { get; set; } = new() { Val = 0.5M };

    [XmlElement("GraceNotesEnabled")]
    public Value<bool> GraceNotesEnabled { get; set; } = new() { Val = false };

    [XmlElement("GraceNotesChance")]
    public Value<decimal> GraceNotesChance { get; set; } = new() { Val = 1 };

    [XmlElement("GraceNotesVelocity")]
    public Value<decimal> GraceNotesVelocity { get; set; } = new() { Val = 0.5M };

    [XmlElement("GraceNotesPosition")]
    public Value<decimal> GraceNotesPosition { get; set; } = new() { Val = -0.200000003M };

    [XmlElement("GraceNotesAmount")]
    public Value<int> GraceNotesAmount { get; set; } = new() { Val = 3 };

    [XmlElement("GraceNotesPitch")]
    public Value<int> GraceNotesPitch { get; set; } = new() { Val = 1 };
}