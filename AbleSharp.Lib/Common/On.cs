using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class On
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("Manual")]
    public Value<bool> Manual { get; set; }

    [XmlElement("AutomationTarget")]
    public AutomationTarget AutomationTarget { get; set; }

    [XmlElement("MidiCCOnOffThresholds")]
    public MidiCCOnOffThresholds MidiCCOnOffThresholds { get; set; }
}