using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipSlotsListWrapper
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; }
}