using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class CrossFadeState
{
    public Value<int> LomId { get; set; }
    public Value<decimal> Manual { get; set; }
    public AutomationTarget AutomationTarget { get; set; }
}