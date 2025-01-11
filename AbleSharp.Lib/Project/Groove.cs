using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Groove
{
    [XmlElement("Name")]
    public Value<string> Name { get; set; } = new Value<string> { Val = "" };
}