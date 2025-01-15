using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlRoot("AutomationEnvelopes")]
public class AutomationEnvelopes
{
    [XmlArray("Envelopes")]
    [XmlArrayItem("AutomationEnvelope")]
    public List<AutomationEnvelope> Envelopes { get; set; } = new();
}