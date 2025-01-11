using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Tempo
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("Manual")]
    public Value<decimal> Manual { get; set; }

    [XmlElement("MidiControllerRange")]
    public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")]
    public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")]
    public ModulationTarget ModulationTarget { get; set; }
}