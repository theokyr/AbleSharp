using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutoColorPicker
{
    [XmlElement("NextColorIndex")]
    public Value<int> NextColorIndex { get; set; } = new() { Val = 9 };
}