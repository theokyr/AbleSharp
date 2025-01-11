using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ScrollerTimePreserver
{
    [XmlElement("LeftTime")]
    public Value<decimal> LeftTime { get; set; }

    [XmlElement("RightTime")]
    public Value<decimal> RightTime { get; set; }
}