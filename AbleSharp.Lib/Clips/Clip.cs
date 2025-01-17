using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlInclude(typeof(MidiClip))]
[XmlInclude(typeof(AudioClip))]
public abstract class Clip
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Time")]
    public decimal Time { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; } = new() { Val = 0 };

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; } = new() { Val = 0 };

    [XmlElement("CurrentStart")]
    public Value<decimal> CurrentStart { get; set; } = new() { Val = 0 };

    [XmlElement("CurrentEnd")]
    public Value<decimal> CurrentEnd { get; set; } = new() { Val = 16 };

    [XmlElement("Loop")]
    public Loop Loop { get; set; } = new();

    [XmlElement("Name")]
    public Value<string> Name { get; set; } = new() { Val = "" };

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; } = new() { Val = "" };

    [XmlElement("Color")]
    public Value<int> Color { get; set; } = new() { Val = 12 };

    [XmlElement("LaunchMode")]
    public Value<int> LaunchMode { get; set; } = new() { Val = 0 };

    [XmlElement("LaunchQuantisation")]
    public Value<int> LaunchQuantisation { get; set; } = new() { Val = 0 };

    [XmlElement("TimeSignature")]
    public TimeSignature TimeSignature { get; set; } = new();

    [XmlElement("Envelopes")]
    public Envelopes Envelopes { get; set; } = new();

    [XmlElement("ScrollerTimePreserver")]
    public ScrollerTimePreserver ScrollerTimePreserver { get; set; } = new();

    [XmlElement("TimeSelection")]
    public TimeSelection TimeSelection { get; set; } = new();

    [XmlElement("Legato")]
    public Value<bool> Legato { get; set; } = new() { Val = false };

    [XmlElement("Ram")]
    public Value<bool> Ram { get; set; } = new() { Val = false };

    [XmlElement("GrooveSettings")]
    public GrooveSettings GrooveSettings { get; set; } = new();

    [XmlElement("Disabled")]
    public Value<bool> Disabled { get; set; } = new() { Val = false };

    [XmlElement("VelocityAmount")]
    public Value<decimal> VelocityAmount { get; set; } = new() { Val = 0 };

    [XmlElement("FollowAction")]
    public FollowAction FollowAction { get; set; } = new();

    [XmlElement("Grid")]
    public Grid Grid { get; set; } = new();

    [XmlElement("FreezeStart")]
    public Value<decimal> FreezeStart { get; set; } = new() { Val = 0 };

    [XmlElement("FreezeEnd")]
    public Value<decimal> FreezeEnd { get; set; } = new() { Val = 0 };

    [XmlElement("IsWarped")]
    public Value<bool> IsWarped { get; set; } = new() { Val = true };

    [XmlElement("TakeId")]
    public Value<int> TakeId { get; set; } = new() { Val = 1 };
}