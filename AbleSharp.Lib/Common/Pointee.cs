using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Pointee
{
    [XmlAttribute("Id")]
    public int Id { get; set; }
}