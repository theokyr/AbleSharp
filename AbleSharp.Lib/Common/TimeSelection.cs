using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TimeSelection
{
    [XmlElement("AnchorTime")]
    public Value<decimal> AnchorTime { get; set; }

    [XmlElement("OtherTime")]
    public Value<decimal> OtherTime { get; set; }
}