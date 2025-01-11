using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class FollowAction
{
    public Value<decimal> FollowTime { get; set; }
    public Value<bool> IsLinked { get; set; }
    public Value<int> LoopIterations { get; set; }
    public Value<int> FollowActionA { get; set; }
    public Value<int> FollowActionB { get; set; }
    public Value<int> FollowChanceA { get; set; }
    public Value<int> FollowChanceB { get; set; }
    public Value<int> JumpIndexA { get; set; }
    public Value<int> JumpIndexB { get; set; }
    public Value<bool> FollowActionEnabled { get; set; }
}