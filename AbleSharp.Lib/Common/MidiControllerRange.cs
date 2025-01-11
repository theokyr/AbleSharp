using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiControllerRange
{
    [XmlElement("Min")]
    public Value<decimal> Min { get; set; }

    [XmlElement("Max")]
    public Value<decimal> Max { get; set; }
}