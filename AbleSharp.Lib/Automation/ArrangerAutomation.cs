using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ArrangerAutomation
{
    [XmlArray("Events")]
    [XmlArrayItem("Event")]
    public List<AutomationEvent> Events { get; set; }

    [XmlElement("AutomationTransformViewState")]
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}