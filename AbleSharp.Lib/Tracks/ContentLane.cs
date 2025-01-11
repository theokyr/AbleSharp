using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ContentLane
{
    public Value<int> Type { get; set; }
    public Value<decimal> Size { get; set; }
    public Value<bool> IsMinimized { get; set; }
}