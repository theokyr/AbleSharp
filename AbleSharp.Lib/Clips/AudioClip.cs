using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AudioClip : Clip
{
    [XmlElement("SampleRef")]
    public SampleRef SampleRef { get; set; }

    [XmlElement("Onsets")]
    public Onsets Onsets { get; set; }

    [XmlElement("WarpMode")]
    public Value<int> WarpMode { get; set; }

    [XmlElement("GranularityTones")]
    public Value<int> GranularityTones { get; set; }

    [XmlElement("GranularityTexture")]
    public Value<int> GranularityTexture { get; set; }

    [XmlElement("FluctuationTexture")]
    public Value<int> FluctuationTexture { get; set; }

    [XmlElement("TransientResolution")]
    public Value<int> TransientResolution { get; set; }

    [XmlElement("TransientLoopMode")]
    public Value<int> TransientLoopMode { get; set; }

    [XmlElement("TransientEnvelope")]
    public Value<int> TransientEnvelope { get; set; }

    [XmlElement("ComplexProFormants")]
    public Value<int> ComplexProFormants { get; set; }

    [XmlElement("ComplexProEnvelope")]
    public Value<int> ComplexProEnvelope { get; set; }

    [XmlElement("Sync")]
    public Value<bool> Sync { get; set; }

    [XmlElement("HiQ")]
    public Value<bool> HiQ { get; set; }

    [XmlElement("Fade")]
    public Value<bool> Fade { get; set; }

    [XmlElement("Fades")]
    public Fades Fades { get; set; }

    [XmlElement("PitchCoarse")]
    public Value<int> PitchCoarse { get; set; }

    [XmlElement("PitchFine")]
    public Value<int> PitchFine { get; set; }

    [XmlElement("SampleVolume")]
    public Value<decimal> SampleVolume { get; set; }

    [XmlArray("WarpMarkers")]
    [XmlArrayItem("WarpMarker")]
    public List<WarpMarker> WarpMarkers { get; set; }

    [XmlElement("SavedWarpMarkersForStretched")]
    public SavedWarpMarkers SavedWarpMarkersForStretched { get; set; }

    [XmlElement("MarkersGenerated")]
    public Value<bool> MarkersGenerated { get; set; }

    [XmlElement("IsSongTempoLeader")]
    public Value<bool> IsSongTempoLeader { get; set; }
}