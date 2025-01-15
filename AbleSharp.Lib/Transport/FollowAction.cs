using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class FollowAction
{
    [XmlElement("FollowTime")]
    public Value<decimal> FollowTime { get; set; } = new() { Val = 4 };

    [XmlElement("IsLinked")]
    public Value<bool> IsLinked { get; set; } = new() { Val = true };

    [XmlElement("LoopIterations")]
    public Value<int> LoopIterations { get; set; } = new() { Val = 1 };

    [XmlElement("FollowActionA")]
    public Value<int> FollowActionA { get; set; } = new() { Val = 4 };

    [XmlElement("FollowActionB")]
    public Value<int> FollowActionB { get; set; } = new() { Val = 0 };

    [XmlElement("FollowChanceA")]
    public Value<int> FollowChanceA { get; set; } = new() { Val = 100 };

    [XmlElement("FollowChanceB")]
    public Value<int> FollowChanceB { get; set; } = new() { Val = 0 };

    [XmlElement("JumpIndexA")]
    public Value<int> JumpIndexA { get; set; } = new() { Val = 1 };

    [XmlElement("JumpIndexB")]
    public Value<int> JumpIndexB { get; set; } = new() { Val = 1 };

    [XmlElement("FollowActionEnabled")]
    public Value<bool> FollowActionEnabled { get; set; } = new() { Val = false };
}