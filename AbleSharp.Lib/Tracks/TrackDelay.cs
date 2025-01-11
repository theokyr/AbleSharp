using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TrackDelay
{
    [XmlElement("Value")]
    public Value<decimal> Value { get; set; }

    [XmlElement("IsValueSampleBased")]
    public Value<bool> IsValueSampleBased { get; set; }
}