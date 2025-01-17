using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SendPreBool
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Value")]
    public bool Value { get; set; }
}