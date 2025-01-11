using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Pointee
{
    [XmlAttribute("Id")]
    public string Id { get; set; }
}