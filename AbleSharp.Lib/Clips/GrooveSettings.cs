using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class GrooveSettings
{
    [XmlElement("GrooveId")]
    public Value<int> GrooveId { get; set; }
}