using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlType("MainTrack")]
public class MainTrack : Track
{
    [XmlElement("Tempo")]
    public Value<decimal> Tempo { get; set; }

    [XmlElement("TimeSignature")]
    public TimeSignature TimeSignature { get; set; }
}