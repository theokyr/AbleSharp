using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ModulationTarget
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("LockEnvelope")]
    public Value<int> LockEnvelope { get; set; }
}