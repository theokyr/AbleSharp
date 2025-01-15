using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TimeWarpAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Breakpoints")]
    public TimeWarpBreakpoints Breakpoints { get; set; } = new();

    [XmlElement("StretchNoteEnd")]
    public Value<bool> StretchNoteEnd { get; set; } = new() { Val = true };

    [XmlElement("PreserveTimeRange")]
    public Value<bool> PreserveTimeRange { get; set; } = new() { Val = true };

    [XmlElement("Quantize")]
    public Value<bool> Quantize { get; set; } = new() { Val = false };
}

public class TimeWarpBreakpoints
{
    [XmlElement("Breakpoint")]
    public List<TimeWarpBreakpoint> Points { get; set; } = new()
    {
        new() { Id = "0", Time = new() { Val = 0 }, Value = new() { Val = 0.5M }, IsActive = new() { Val = true } },
        new() { Id = "1", Time = new() { Val = 0.5M }, Value = new() { Val = 0.5M }, IsActive = new() { Val = false } },
        new() { Id = "2", Time = new() { Val = 1 }, Value = new() { Val = 0.5M }, IsActive = new() { Val = true } }
    };
}

public class TimeWarpBreakpoint
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Value")]
    public Value<decimal> Value { get; set; }

    [XmlElement("Time")]
    public Value<decimal> Time { get; set; }

    [XmlElement("Control1X")]
    public Value<decimal> Control1X { get; set; } = new() { Val = 0.5M };

    [XmlElement("Control1Y")]
    public Value<decimal> Control1Y { get; set; } = new() { Val = 0.5M };

    [XmlElement("Control2X")]
    public Value<decimal> Control2X { get; set; } = new() { Val = 0.5M };

    [XmlElement("Control2Y")]
    public Value<decimal> Control2Y { get; set; } = new() { Val = 0.5M };

    [XmlElement("IsActive")]
    public Value<bool> IsActive { get; set; }
}