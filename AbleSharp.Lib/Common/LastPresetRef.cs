using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class LastPresetRef
{
    [XmlElement("Value")]
    public string Value { get; set; }
}