using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationEnvelope
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("EnvelopeTarget")]
    public EnvelopeTarget EnvelopeTarget { get; set; }

    [XmlElement("Automation")]
    public Automation Automation { get; set; }
}