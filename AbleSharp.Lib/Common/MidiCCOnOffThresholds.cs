using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiCCOnOffThresholds
{
    [XmlElement("Min")]
    public Value<int> Min { get; set; }

    [XmlElement("Max")]
    public Value<int> Max { get; set; }
}