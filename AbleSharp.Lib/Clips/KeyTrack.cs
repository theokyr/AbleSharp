using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class KeyTrack
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlArray("Notes")]
    [XmlArrayItem("MidiNoteEvent")]
    public List<MidiNoteEvent> Notes { get; set; }

    [XmlElement("MidiKey")]
    public Value<int> MidiKey { get; set; }
}