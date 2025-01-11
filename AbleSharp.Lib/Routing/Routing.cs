using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Routing
{
    [XmlElement("Target")]
    public Value<string> Target { get; set; }

    [XmlElement("UpperDisplayString")]
    public Value<string> UpperDisplayString { get; set; }

    [XmlElement("LowerDisplayString")]
    public Value<string> LowerDisplayString { get; set; }

    [XmlElement("MpeSettings")]
    public MpeSettings MpeSettings { get; set; }
}