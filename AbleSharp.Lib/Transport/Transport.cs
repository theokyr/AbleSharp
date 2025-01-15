using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Transport
{
    [XmlElement("PhaseNudgeTempo")]
    public Value<decimal> PhaseNudgeTempo { get; set; } = new() { Val = 10 };

    [XmlElement("LoopOn")]
    public Value<bool> LoopOn { get; set; } = new() { Val = false };

    [XmlElement("LoopStart")]
    public Value<decimal> LoopStart { get; set; } = new() { Val = 8 };

    [XmlElement("LoopLength")]
    public Value<decimal> LoopLength { get; set; } = new() { Val = 16 };

    [XmlElement("LoopIsSongStart")]
    public Value<bool> LoopIsSongStart { get; set; } = new() { Val = false };

    [XmlElement("CurrentTime")]
    public Value<decimal> CurrentTime { get; set; } = new() { Val = 0 };

    [XmlElement("PunchIn")]
    public Value<bool> PunchIn { get; set; } = new() { Val = false };

    [XmlElement("PunchOut")]
    public Value<bool> PunchOut { get; set; } = new() { Val = false };

    [XmlElement("MetronomeTickDuration")]
    public Value<decimal> MetronomeTickDuration { get; set; } = new() { Val = 0 };

    [XmlElement("DrawMode")]
    public Value<bool> DrawMode { get; set; } = new() { Val = false };
}