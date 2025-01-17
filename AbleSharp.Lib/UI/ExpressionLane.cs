using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ExpressionLane
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Type")]
    public Value<int> Type { get; set; }

    [XmlElement("Size")]
    public Value<decimal> Size { get; set; }

    [XmlElement("IsMinimized")]
    public Value<bool> IsMinimized { get; set; }
}