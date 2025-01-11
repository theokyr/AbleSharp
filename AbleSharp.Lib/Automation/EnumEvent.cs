using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class EnumEvent : AutomationEventBase
{
    [XmlAttribute("Value")]
    public int Value { get; set; }
}