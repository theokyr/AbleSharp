using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipEnvelope
{
    public Value<string> Id { get; set; }
    public EnvelopeTarget EnvelopeTarget { get; set; }
    public Automation Automation { get; set; }
    public LoopSlot LoopSlot { get; set; }
    public ScrollerTimePreserver ScrollerTimePreserver { get; set; }
}