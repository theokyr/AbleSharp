using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Envelopes
{
    [XmlElement("Envelopes")]
    public List<ClipEnvelope>? EnvelopeCollection { get; set; }
}