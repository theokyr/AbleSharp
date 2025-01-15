using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class GroovePool
{
    [XmlAttribute("LomId")]
    public int LomId { get; set; } = 0;

    [XmlArray("Grooves")]
    [XmlArrayItem("Groove")]
    public List<Groove> Grooves { get; set; } = new();
}