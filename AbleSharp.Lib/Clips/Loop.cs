using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Loop
{
    [XmlElement("LoopStart")]
    public Value<decimal> LoopStart { get; set; }

    [XmlElement("LoopEnd")]
    public Value<decimal> LoopEnd { get; set; }

    [XmlElement("StartRelative")]
    public Value<decimal> StartRelative { get; set; }

    [XmlElement("LoopOn")]
    public Value<bool> LoopOn { get; set; }

    [XmlElement("OutMarker")]
    public Value<decimal> OutMarker { get; set; }

    [XmlElement("HiddenLoopStart")]
    public Value<decimal> HiddenLoopStart { get; set; }

    [XmlElement("HiddenLoopEnd")]
    public Value<decimal> HiddenLoopEnd { get; set; }
}