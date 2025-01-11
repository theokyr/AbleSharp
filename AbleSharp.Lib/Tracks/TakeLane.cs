using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TakeLane
{
    public Value<string> Id { get; set; }
    public Value<decimal> Height { get; set; }
    public Value<bool> IsContentSelectedInDocument { get; set; }
    public Value<string> Name { get; set; }
    public Value<string> Annotation { get; set; }
    public ClipAutomation ClipAutomation { get; set; }
}