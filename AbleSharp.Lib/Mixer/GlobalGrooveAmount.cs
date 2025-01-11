using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class GlobalGrooveAmount
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}