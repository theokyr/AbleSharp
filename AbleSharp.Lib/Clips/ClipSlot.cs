using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipSlot
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("ClipSlot")]
    public ClipSlotValue ClipData { get; set; }

    [XmlElement("HasStop")]
    public Value<bool> HasStop { get; set; }

    [XmlElement("NeedRefreeze")]
    public Value<bool> NeedRefreeze { get; set; }
}