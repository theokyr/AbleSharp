using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipSlotValue
{
    [XmlElement("Value")]
    public string Value { get; set; }

    [XmlElement("Clip")]
    public Clip Clip { get; set; }
}