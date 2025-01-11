using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SendsListWrapper
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; }
}