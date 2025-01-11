using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class WarpMarker
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("SecTime")]
    public Value<decimal> SecTime { get; set; }

    [XmlElement("BeatTime")]
    public Value<decimal> BeatTime { get; set; }
}