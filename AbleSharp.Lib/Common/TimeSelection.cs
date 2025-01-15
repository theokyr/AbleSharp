using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TimeSelection
{
    [XmlElement("AnchorTime")]
    public Value<string> AnchorTime { get; set; } = new() { Val = "Invalid" };

    [XmlElement("OtherTime")]
    public Value<string> OtherTime { get; set; } = new() { Val = "Invalid" };
}