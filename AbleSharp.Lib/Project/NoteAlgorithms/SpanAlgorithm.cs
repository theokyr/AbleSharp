using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SpanAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Mode")]
    public Value<int> Mode { get; set; } = new() { Val = 1 };

    [XmlElement("ChordThreshold")]
    public Value<int> ChordThreshold { get; set; } = new() { Val = 10 };

    [XmlElement("LengthVariation")]
    public Value<int> LengthVariation { get; set; } = new() { Val = 0 };

    [XmlElement("LengthOffset")]
    public Value<int> LengthOffset { get; set; } = new() { Val = 0 };

    [XmlElement("FixedLength")]
    public Value<int> FixedLength { get; set; } = new() { Val = 0 };
}