using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class StacksAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Sequence")]
    public ChordSequence Sequence { get; set; } = new();
}

public class ChordSequence
{
    [XmlElement("Chord")]
    public List<Chord> Chords { get; set; } = new()
    {
        new Chord
        {
            Id = "0",
            RootDegree = new Value<int> { Val = 0 },
            Octave = new Value<int> { Val = 3 },
            RootPitch = new Value<int> { Val = 60 },
            Inversion = new Value<int> { Val = 0 },
            RuleNumber = new Value<int> { Val = 0 },
            Duration = new Value<int> { Val = 1 },
            Offset = new Value<int> { Val = 0 }
        }
    };
}

public class Chord
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("RootDegree")]
    public Value<int> RootDegree { get; set; }

    [XmlElement("Octave")]
    public Value<int> Octave { get; set; }

    [XmlElement("RootPitch")]
    public Value<int> RootPitch { get; set; }

    [XmlElement("Inversion")]
    public Value<int> Inversion { get; set; }

    [XmlElement("RuleNumber")]
    public Value<int> RuleNumber { get; set; }

    [XmlElement("Duration")]
    public Value<int> Duration { get; set; }

    [XmlElement("Offset")]
    public Value<int> Offset { get; set; }
}