using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class BoolEvent : AutomationEventBase
{
    [XmlAttribute("Value")]
    public bool Value { get; set; }
}