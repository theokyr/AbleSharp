using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClientSize
{
    public Value<int> X { get; set; }
    public Value<int> Y { get; set; }
}