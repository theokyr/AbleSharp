using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ScaleInformation
{
    public Value<int> RootNote { get; set; }
    public Value<string> Name { get; set; }
}