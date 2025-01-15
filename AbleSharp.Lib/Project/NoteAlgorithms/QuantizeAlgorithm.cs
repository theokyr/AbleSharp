using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class QuantizeAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Reference")]
    public Value<int> Reference { get; set; } = new() { Val = 0 };

    [XmlElement("TripletReference")]
    public Value<bool> TripletReference { get; set; } = new() { Val = false };

    [XmlElement("QuantizeNoteStarts")]
    public Value<bool> QuantizeNoteStarts { get; set; } = new() { Val = true };

    [XmlElement("QuantizeNoteEnds")]
    public Value<bool> QuantizeNoteEnds { get; set; } = new() { Val = false };

    [XmlElement("Amount")]
    public Value<int> Amount { get; set; } = new() { Val = 100 };
}