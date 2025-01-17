using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class NoteAlgorithms
{
    [XmlElement("ArpeggiateAlgorithm")]
    public ArpeggiateAlgorithm ArpeggiateAlgorithm { get; set; } = new();

    [XmlElement("SpanAlgorithm")]
    public SpanAlgorithm SpanAlgorithm { get; set; } = new();

    [XmlElement("ConnectAlgorithm")]
    public ConnectAlgorithm ConnectAlgorithm { get; set; } = new();

    [XmlElement("OrnamentAlgorithm")]
    public OrnamentAlgorithm OrnamentAlgorithm { get; set; } = new();

    [XmlElement("RecombineAlgorithm")]
    public RecombineAlgorithm RecombineAlgorithm { get; set; } = new();

    [XmlElement("QuantizeAlgorithm")]
    public QuantizeAlgorithm QuantizeAlgorithm { get; set; } = new();

    [XmlElement("StrumAlgorithm")]
    public StrumAlgorithm StrumAlgorithm { get; set; } = new();

    [XmlElement("TimeWarpAlgorithm")]
    public TimeWarpAlgorithm TimeWarpAlgorithm { get; set; } = new();

    [XmlElement("RhythmAlgorithm")]
    public RhythmAlgorithm RhythmAlgorithm { get; set; } = new();

    [XmlElement("StacksAlgorithm")]
    public StacksAlgorithm StacksAlgorithm { get; set; } = new();

    // TODO Fixme
    // [XmlElement("ShapeAlgorithm")]
    // public ShapeAlgorithm ShapeAlgorithm { get; set; } = new();

    [XmlElement("SeedAlgorithm")]
    public SeedAlgorithm SeedAlgorithm { get; set; } = new();
}