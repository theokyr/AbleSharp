using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiNoteEvent
{
    [XmlAttribute("Time")]
    public decimal Time { get; set; }

    [XmlAttribute("Duration")]
    public decimal Duration { get; set; }

    [XmlAttribute("Velocity")]
    public int Velocity { get; set; }

    [XmlAttribute("VelocityDeviation")]
    public int VelocityDeviation { get; set; }

    [XmlAttribute("OffVelocity")]
    public int OffVelocity { get; set; }

    [XmlAttribute("Probability")]
    public decimal Probability { get; set; }

    [XmlAttribute("IsEnabled")]
    public bool IsEnabled { get; set; }

    [XmlAttribute("NoteId")]
    public int NoteId { get; set; }
}