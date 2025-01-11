using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ParametersListWrapper
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; }
}