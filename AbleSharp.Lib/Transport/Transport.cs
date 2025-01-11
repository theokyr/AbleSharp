using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Transport
{
    [XmlElement("PhaseNudgeTempo")]
    public Value<decimal> PhaseNudgeTempo { get; set; }

    [XmlElement("LoopOn")]
    public Value<bool> LoopOn { get; set; }

    [XmlElement("LoopStart")]
    public Value<decimal> LoopStart { get; set; }

    [XmlElement("LoopLength")]
    public Value<decimal> LoopLength { get; set; }

    [XmlElement("LoopIsSongStart")]
    public Value<bool> LoopIsSongStart { get; set; }

    [XmlElement("CurrentTime")]
    public Value<decimal> CurrentTime { get; set; }

    [XmlElement("PunchIn")]
    public Value<bool> PunchIn { get; set; }

    [XmlElement("PunchOut")]
    public Value<bool> PunchOut { get; set; }

    [XmlElement("MetronomeTickDuration")]
    public Value<decimal> MetronomeTickDuration { get; set; }

    [XmlElement("DrawMode")]
    public Value<bool> DrawMode { get; set; }
}