using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Notes
{
    [XmlArray("KeyTracks")]
    [XmlArrayItem("KeyTrack")]
    public List<KeyTrack> KeyTracks { get; set; } = new();

    [XmlElement("PerNoteEventStore")]
    public PerNoteEventStore PerNoteEventStore { get; set; }

    [XmlArray("NoteProbabilityGroups")]
    [XmlArrayItem("NoteProbabilityGroup")]
    public List<NoteProbabilityGroup> NoteProbabilityGroups { get; set; } = new();

    [XmlElement("ProbabilityGroupIdGenerator")]
    public ProbabilityGroupIdGenerator ProbabilityGroupIdGenerator { get; set; }

    [XmlElement("NoteIdGenerator")]
    public NoteIdGenerator NoteIdGenerator { get; set; }
}