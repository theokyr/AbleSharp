using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ConnectAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Tie")]
    public Value<int> Tie { get; set; } = new() { Val = 0 };

    [XmlElement("Density")]
    public Value<int> Density { get; set; } = new() { Val = 1 };

    [XmlElement("Spread")]
    public Value<int> Spread { get; set; } = new() { Val = 0 };

    [XmlElement("Rate")]
    public Value<int> Rate { get; set; } = new() { Val = 0 };
}