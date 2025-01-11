using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Recorder
{
    [XmlElement("IsArmed")]
    public Value<bool> IsArmed { get; set; }

    [XmlElement("TakeCounter")]
    public Value<int> TakeCounter { get; set; }
}