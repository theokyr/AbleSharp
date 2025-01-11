using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Parameter
{
    public Value<string> Id { get; set; }
    public Value<decimal> Value { get; set; }
    public MidiControllerRange MidiControllerRange { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}