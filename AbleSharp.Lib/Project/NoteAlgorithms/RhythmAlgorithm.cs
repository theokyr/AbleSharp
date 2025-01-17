using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class RhythmAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Density")]
    public Value<int> Density { get; set; } = new() { Val = 4 };

    [XmlElement("Repetitions")]
    public Value<int> Repetitions { get; set; } = new() { Val = 1 };

    [XmlElement("Pattern")]
    public Value<int> Pattern { get; set; } = new() { Val = 14 };

    [XmlElement("PatternLength")]
    public Value<int> PatternLength { get; set; } = new() { Val = 8 };

    [XmlElement("Pitch")]
    public Value<int> Pitch { get; set; } = new() { Val = 72 };

    [XmlElement("Velocity")]
    public Value<int> Velocity { get; set; } = new() { Val = 100 };

    [XmlElement("Accent")]
    public Value<int> Accent { get; set; } = new() { Val = 127 };

    [XmlElement("Period")]
    public Value<int> Period { get; set; } = new() { Val = 4 };

    [XmlElement("Offset")]
    public Value<int> Offset { get; set; } = new() { Val = 0 };

    [XmlElement("Shift")]
    public Value<int> Shift { get; set; } = new() { Val = 0 };

    [XmlElement("StepDuration")]
    public Value<int> StepDuration { get; set; } = new() { Val = 7 };

    [XmlElement("Split")]
    public Value<int> Split { get; set; } = new() { Val = 0 };
}