using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class StrumAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("ChordThreshold")]
    public Value<int> ChordThreshold { get; set; } = new() { Val = 0 };

    [XmlElement("StrumLow")]
    public Value<int> StrumLow { get; set; } = new() { Val = 0 };

    [XmlElement("StrumHigh")]
    public Value<int> StrumHigh { get; set; } = new() { Val = 0 };

    [XmlElement("TensionAmount")]
    public Value<int> TensionAmount { get; set; } = new() { Val = 0 };
}