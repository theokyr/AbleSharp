using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class RecombineAlgorithm
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("PermutePitches")]
    public Value<bool> PermutePitches { get; set; } = new() { Val = true };

    [XmlElement("PermuteDurations")]
    public Value<bool> PermuteDurations { get; set; } = new() { Val = true };

    [XmlElement("PermuteVelocities")]
    public Value<bool> PermuteVelocities { get; set; } = new() { Val = true };

    [XmlElement("Shuffle")]
    public Value<bool> Shuffle { get; set; } = new() { Val = false };

    [XmlElement("Mirror")]
    public Value<bool> Mirror { get; set; } = new() { Val = false };

    [XmlElement("RotateAmount")]
    public Value<int> RotateAmount { get; set; } = new() { Val = 0 };
}