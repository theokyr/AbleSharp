using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationLane
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    public Value<int> SelectedDevice { get; set; }
    public Value<int> SelectedEnvelope { get; set; }
    public Value<bool> IsContentSelectedInDocument { get; set; }
    public Value<decimal> LaneHeight { get; set; }
}