using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Speaker
{
    public Value<int> LomId { get; set; }
    public Value<bool> Manual { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public MidiCCOnOffThresholds MidiCCOnOffThresholds { get; set; }
}