using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class GroupTrackSlot
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }
}