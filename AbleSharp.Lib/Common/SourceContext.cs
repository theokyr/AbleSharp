using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SourceContext
{
    [XmlElement("Value")]
    public string Value { get; set; }
}