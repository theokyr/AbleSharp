using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SampleRef
{
    [XmlElement("FileRef")]
    public FileRef FileRef { get; set; }

    [XmlElement("LastModDate")]
    public Value<long> LastModDate { get; set; }

    [XmlElement("SourceContext")]
    public SourceContext SourceContext { get; set; }

    [XmlElement("SampleUsageHint")]
    public Value<int> SampleUsageHint { get; set; }

    [XmlElement("DefaultDuration")]
    public Value<long> DefaultDuration { get; set; }

    [XmlElement("DefaultSampleRate")]
    public Value<int> DefaultSampleRate { get; set; }
}