using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ScrollPosition
{
    [XmlAttribute("X")]
    public int X { get; set; } = 0;

    [XmlAttribute("Y")]
    public int Y { get; set; } = 0;
}