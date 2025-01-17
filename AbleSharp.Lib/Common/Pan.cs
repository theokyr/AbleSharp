using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Pan
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

public class SplitStereoPanL
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("Manual")]
    public Value<decimal> Manual { get; set; } = -1;

    [XmlElement("MidiControllerRange")]
    public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")]
    public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")]
    public ModulationTarget ModulationTarget { get; set; }
}

public class SplitStereoPanR
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("Manual")]
    public Value<decimal> Manual { get; set; } = 1;

    [XmlElement("MidiControllerRange")]
    public MidiControllerRange MidiControllerRange { get; set; }

    [XmlElement("AutomationTarget")]
    public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("ModulationTarget")]
    public ModulationTarget ModulationTarget { get; set; }
}