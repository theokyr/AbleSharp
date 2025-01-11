using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SendPreBool
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlAttribute("Value")]
    public bool Value { get; set; }
}