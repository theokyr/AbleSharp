using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MpeSettings
{
    [XmlElement("ZoneType")]
    public Value<int> ZoneType { get; set; }

    [XmlElement("FirstNoteChannel")]
    public Value<int> FirstNoteChannel { get; set; }

    [XmlElement("LastNoteChannel")]
    public Value<int> LastNoteChannel { get; set; }
}