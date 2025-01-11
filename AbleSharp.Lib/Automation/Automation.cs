using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Automation
{
    [XmlArray("Events")]
    [XmlArrayItem("FloatEvent", typeof(FloatEvent))]
    [XmlArrayItem("EnumEvent", typeof(EnumEvent))]
    [XmlArrayItem("BoolEvent", typeof(BoolEvent))]
    public List<AutomationEventBase> Events { get; set; }

    [XmlElement("AutomationTransformViewState")]
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}