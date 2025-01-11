using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class DevicesListWrapper
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; }
}