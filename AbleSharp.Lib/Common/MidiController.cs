using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiController
{
    public Value<string> Id { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}