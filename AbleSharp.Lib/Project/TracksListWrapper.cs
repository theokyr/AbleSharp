using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TracksListWrapper
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; } = 0;
}