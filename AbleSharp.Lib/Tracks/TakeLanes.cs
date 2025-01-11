using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TakeLanes
{
    [XmlElement("TakeLanes")]
    public List<TakeLane> LaneCollection { get; set; }

    public Value<bool> AreTakeLanesFolded { get; set; }
}