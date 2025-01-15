using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ShapeAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("ShapeLevels")]
    public ShapeLevels ShapeLevels { get; set; } = new();

    [XmlElement("ShapePresets")]
    public Value<int> ShapePresets { get; set; } = new() { Val = 2 };

    [XmlElement("Rate")]
    public Value<int> Rate { get; set; } = new() { Val = 0 };

    [XmlElement("Tie")]
    public Value<int> Tie { get; set; } = new() { Val = 0 };

    [XmlElement("Density")]
    public Value<int> Density { get; set; } = new() { Val = 1 };

    [XmlElement("MinPitch")]
    public Value<int> MinPitch { get; set; } = new() { Val = 60 };

    [XmlElement("MaxPitch")]
    public Value<int> MaxPitch { get; set; } = new() { Val = 84 };

    [XmlElement("PitchVariation")]
    public Value<int> PitchVariation { get; set; } = new() { Val = 0 };
}

public class ShapeLevels
{
    [XmlElement("RemoteableFloat")]
    public List<RemoteableFloat> Levels { get; set; } = CreateDefaultLevels();

    private static List<RemoteableFloat> CreateDefaultLevels()
    {
        var levels = new List<RemoteableFloat>();
        var values = new decimal[] { 
            0, 0.05000000075M, 0.1000000015M, 0.150000006M, 0.200000003M,
            0.25M, 0.3000000119M, 0.349999994M, 0.400000006M, 0.4499999881M,
            0.5M, 0.5500000119M, 0.6000000238M, 0.6499999762M, 0.6999999881M,
            0.75M, 0.8000000119M, 0.8500000238M, 0.8999999762M, 0.9499999881M, 1
        };

        for (int i = 0; i < values.Length; i++)
        {
            levels.Add(new RemoteableFloat 
            { 
                Id = i.ToString(), 
                Value = new() { Val = values[i] }
            });
        }
        return levels;
    }
}

public class RemoteableFloat
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlAttribute("Value")]
    public Value<decimal> Value { get; set; }
}