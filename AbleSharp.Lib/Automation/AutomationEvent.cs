using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationEvent
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Time")]
    public decimal Time { get; set; }

    [XmlAttribute("Value")]
    public string Value { get; set; }
}

[XmlInclude(typeof(FloatEvent))]
[XmlInclude(typeof(EnumEvent))]
[XmlInclude(typeof(BoolEvent))]
public abstract class AutomationEventBase
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Time")]
    public decimal Time { get; set; }
}