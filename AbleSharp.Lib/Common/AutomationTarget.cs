using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationTarget
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("LockEnvelope")]
    public Value<int> LockEnvelope { get; set; }
}