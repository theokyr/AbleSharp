using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class RemoteableTimeSignature
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Numerator")]
    public Value<int> Numerator { get; set; }

    [XmlElement("Denominator")]
    public Value<int> Denominator { get; set; }

    [XmlElement("Time")]
    public Value<decimal> Time { get; set; }
}