using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class VideoWindowRect
{
    [XmlAttribute("Top")]
    public int Top { get; set; } = -2147483648;

    [XmlAttribute("Left")]
    public int Left { get; set; } = -2147483648;

    [XmlAttribute("Bottom")]
    public int Bottom { get; set; } = -2147483648;

    [XmlAttribute("Right")]
    public int Right { get; set; } = -2147483648;
}