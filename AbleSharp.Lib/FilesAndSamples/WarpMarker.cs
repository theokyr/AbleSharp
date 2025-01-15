using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class WarpMarker
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("SecTime")]
    public string SecTime { get; set; } = "Invalid";

    [XmlAttribute("BeatTime")]
    public string BeatTime { get; set; } = "Invalid";
}