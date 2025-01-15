using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SeedAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("NotesDensity")]
    public Value<decimal> NotesDensity { get; set; } = new() { Val = 0.5M };

    [XmlElement("MinPitch")]
    public Value<int> MinPitch { get; set; } = new() { Val = 60 };

    [XmlElement("MaxPitch")]
    public Value<int> MaxPitch { get; set; } = new() { Val = 84 };

    [XmlElement("MinDuration")]
    public Value<int> MinDuration { get; set; } = new() { Val = -6 };

    [XmlElement("MaxDuration")]
    public Value<int> MaxDuration { get; set; } = new() { Val = -3 };

    [XmlElement("MinVelocity")]
    public Value<int> MinVelocity { get; set; } = new() { Val = 30 };

    [XmlElement("MaxVelocity")]
    public Value<int> MaxVelocity { get; set; } = new() { Val = 100 };

    [XmlElement("VerticalLimit")]
    public Value<int> VerticalLimit { get; set; } = new() { Val = 4 };
}