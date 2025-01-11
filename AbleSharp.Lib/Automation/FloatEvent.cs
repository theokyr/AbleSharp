using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class FloatEvent : AutomationEventBase
{
    [XmlAttribute("Value")]
    public decimal Value { get; set; }
}