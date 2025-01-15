using System.Xml.Serialization;

namespace AbleSharp.Lib;

/// <summary>
/// Represents a single Clip in Ableton. 
/// 
/// `Time` typically is the arrangement offset: how far into the arrangement this clip starts.
/// `CurrentStart` is the local start time within the clip data.
/// `CurrentEnd` is the local end time within the clip data.
/// </summary>
[XmlInclude(typeof(MidiClip))]
[XmlInclude(typeof(AudioClip))]
public abstract class Clip
{
    /// <summary>
    /// Unique ID for this clip
    /// </summary>
    [XmlAttribute("Id")]
    public string Id { get; set; }

    /// <summary>
    /// Arrangement offset (in beats, or seconds, etc.) 
    /// where this clip is placed on the timeline.
    /// </summary>
    [XmlAttribute("Time")]
    public decimal Time { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; }

    /// <summary>
    /// Local in-point within the clip’s own data.
    /// If the clip is untrimmed, CurrentStart often equals 0.
    /// Some older sets may store the entire offset here as well.
    /// </summary>
    [XmlElement("CurrentStart")]
    public Value<decimal> CurrentStart { get; set; }

    /// <summary>
    /// Local out-point within the clip’s data.
    /// </summary>
    [XmlElement("CurrentEnd")]
    public Value<decimal> CurrentEnd { get; set; }

    [XmlElement("Loop")]
    public Loop Loop { get; set; }

    [XmlElement("Name")]
    public Value<string> Name { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("Color")]
    public Value<int> Color { get; set; }

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