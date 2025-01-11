using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiClip : Clip
{
    [XmlArray("KeyTracks")]
    [XmlArrayItem("KeyTrack")]
    public List<KeyTrack> KeyTracks { get; set; }

    [XmlElement("PerNoteEventStore")]
    public PerNoteEventStore PerNoteEventStore { get; set; }

    [XmlArray("NoteProbabilityGroups")]
    [XmlArrayItem("NoteProbabilityGroup")]
    public List<NoteProbabilityGroup> NoteProbabilityGroups { get; set; }

    [XmlElement("ProbabilityGroupIdGenerator")]
    public ProbabilityGroupIdGenerator ProbabilityGroupIdGenerator { get; set; }

    [XmlElement("NoteIdGenerator")]
    public NoteIdGenerator NoteIdGenerator { get; set; }

    [XmlElement("BankSelectCoarse")]
    public Value<int> BankSelectCoarse { get; set; }

    [XmlElement("BankSelectFine")]
    public Value<int> BankSelectFine { get; set; }

    [XmlElement("ProgramChange")]
    public Value<int> ProgramChange { get; set; }

    [XmlElement("NoteEditorFoldInZoom")]
    public Value<int> NoteEditorFoldInZoom { get; set; }

    [XmlElement("NoteEditorFoldInScroll")]
    public Value<int> NoteEditorFoldInScroll { get; set; }

    [XmlElement("NoteEditorFoldOutZoom")]
    public Value<int> NoteEditorFoldOutZoom { get; set; }

    [XmlElement("NoteEditorFoldOutScroll")]
    public Value<int> NoteEditorFoldOutScroll { get; set; }

    [XmlElement("NoteEditorFoldScaleZoom")]
    public int NoteEditorFoldScaleZoom { get; set; }

    [XmlElement("NoteEditorFoldScaleScroll")]
    public int NoteEditorFoldScaleScroll { get; set; }

    [XmlElement("ScaleInformation")]
    public ScaleInformation ScaleInformation { get; set; }

    [XmlElement("IsInKey")]
    public bool IsInKey { get; set; }

    [XmlElement("NoteSpellingPreference")]
    public int NoteSpellingPreference { get; set; }

    [XmlElement("AccidentalSpellingPreference")]
    public int AccidentalSpellingPreference { get; set; }

    [XmlElement("PreferFlatRootNote")]
    public bool PreferFlatRootNote { get; set; }

    [XmlElement("ExpressionGrid")]
    public ExpressionGrid ExpressionGrid { get; set; }
}