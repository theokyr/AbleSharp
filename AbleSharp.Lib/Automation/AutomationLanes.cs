using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationLanes
{
    [XmlArray("AutomationLanes")]
    [XmlArrayItem("AutomationLane")]
    public List<AutomationLane> Lanes { get; set; }

    [XmlElement("AreAdditionalAutomationLanesFolded")]
    public Value<bool> AreAdditionalAutomationLanesFolded { get; set; }
}