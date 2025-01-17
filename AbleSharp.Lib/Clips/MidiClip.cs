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
    public Value<int> NoteEditorFoldInZoom { get; set; } = new() { Val = -1 };

    [XmlElement("NoteEditorFoldInScroll")]
    public Value<int> NoteEditorFoldInScroll { get; set; } = new() { Val = 0 };

    [XmlElement("NoteEditorFoldOutZoom")]
    public Value<int> NoteEditorFoldOutZoom { get; set; } = new() { Val = -1 };

    [XmlElement("NoteEditorFoldOutScroll")]
    public Value<int> NoteEditorFoldOutScroll { get; set; } = new() { Val = 0 };

    [XmlElement("NoteEditorFoldScaleZoom")]
    public Value<int> NoteEditorFoldScaleZoom { get; set; } = new() { Val = -1 };

    [XmlElement("NoteEditorFoldScaleScroll")]
    public Value<int> NoteEditorFoldScaleScroll { get; set; } = new() { Val = 0 };

    [XmlElement("ScaleInformation")]
    public ScaleInformation ScaleInformation { get; set; }

    [XmlElement("IsInKey")]
    public Value<bool> IsInKey { get; set; }

    [XmlElement("NoteSpellingPreference")]
    public Value<int> NoteSpellingPreference { get; set; }

    [XmlElement("AccidentalSpellingPreference")]
    public Value<int> AccidentalSpellingPreference { get; set; }

    [XmlElement("PreferFlatRootNote")]
    public Value<bool> PreferFlatRootNote { get; set; }

    [XmlElement("ExpressionGrid")]
    public ExpressionGrid ExpressionGrid { get; set; }

    [XmlElement("Notes")]
    public Notes Notes { get; set; }
}