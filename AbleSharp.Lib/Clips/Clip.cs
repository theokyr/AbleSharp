using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlInclude(typeof(MidiClip))]
[XmlInclude(typeof(AudioClip))]
public abstract class Clip
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlAttribute("Time")]
    public decimal Time { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; }

    [XmlElement("CurrentStart")]
    public Value<decimal> CurrentStart { get; set; }

    [XmlElement("CurrentEnd")]
    public Value<decimal> CurrentEnd { get; set; }

    [XmlElement("Loop")]
    public Loop Loop { get; set; }

    [XmlElement("Name")]
    public Value<string> Name { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("Color")]
    public Value<string> Color { get; set; }

    [XmlElement("LaunchMode")]
    public Value<int> LaunchMode { get; set; }

    [XmlElement("LaunchQuantisation")]
    public Value<int> LaunchQuantisation { get; set; }

    [XmlElement("TimeSignature")]
    public TimeSignature TimeSignature { get; set; }

    [XmlElement("Envelopes")]
    public Envelopes Envelopes { get; set; }

    [XmlElement("ScrollerTimePreserver")]
    public ScrollerTimePreserver ScrollerTimePreserver { get; set; }

    [XmlElement("TimeSelection")]
    public TimeSelection TimeSelection { get; set; }

    [XmlElement("Legato")]
    public Value<bool> Legato { get; set; }

    [XmlElement("Ram")]
    public Value<bool> Ram { get; set; }

    [XmlElement("GrooveSettings")]
    public GrooveSettings GrooveSettings { get; set; }

    [XmlElement("Disabled")]
    public Value<bool> Disabled { get; set; }

    [XmlElement("VelocityAmount")]
    public Value<decimal> VelocityAmount { get; set; }

    [XmlElement("FollowAction")]
    public FollowAction FollowAction { get; set; }

    [XmlElement("Grid")]
    public Grid Grid { get; set; }

    [XmlElement("FreezeStart")]
    public Value<decimal> FreezeStart { get; set; }

    [XmlElement("FreezeEnd")]
    public Value<decimal> FreezeEnd { get; set; }

    [XmlElement("IsWarped")]
    public Value<bool> IsWarped { get; set; }

    [XmlElement("TakeId")]
    public Value<int> TakeId { get; set; }
}