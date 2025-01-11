using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class EnvelopeTarget
{
    [XmlElement("PointeeId")]
    public Value<string> PointeeId { get; set; }
}