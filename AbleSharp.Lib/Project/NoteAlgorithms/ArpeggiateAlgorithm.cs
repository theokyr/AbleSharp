using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ArpeggiateAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Rate")]
    public Value<int> Rate { get; set; } = new() { Val = 10 };

    [XmlElement("Steps")]
    public Value<int> Steps { get; set; } = new() { Val = 2 };

    [XmlElement("Distance")]
    public Value<int> Distance { get; set; } = new() { Val = 12 };

    [XmlElement("Gate")]
    public Value<int> Gate { get; set; } = new() { Val = 1 };

    [XmlElement("Style")]
    public Value<int> Style { get; set; } = new() { Val = 0 };
}