using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Locator
{
    public Value<string> Time { get; set; }
    public Value<string> Name { get; set; }
    public Value<string> Annotation { get; set; }
}