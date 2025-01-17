using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationEnvelope
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("EnvelopeTarget")]
    public EnvelopeTarget EnvelopeTarget { get; set; }

    [XmlElement("Automation")]
    public Automation Automation { get; set; }
}