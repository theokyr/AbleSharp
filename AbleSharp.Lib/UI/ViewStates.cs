using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ViewStates
{
    [XmlElement("MixerInArrangement")]
    public Value<int> MixerInArrangement { get; set; } = new() { Val = 0 };

    [XmlElement("ArrangerMixerIO")]
    public Value<int> ArrangerMixerIO { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerMixerSends")]
    public Value<int> ArrangerMixerSends { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerMixerReturns")]
    public Value<int> ArrangerMixerReturns { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerMixerVolume")]
    public Value<int> ArrangerMixerVolume { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerMixerTrackOptions")]
    public Value<int> ArrangerMixerTrackOptions { get; set; } = new() { Val = 0 };

    [XmlElement("ArrangerMixerCrossFade")]
    public Value<int> ArrangerMixerCrossFade { get; set; } = new() { Val = 0 };

    [XmlElement("ArrangerMixerTrackPerformanceImpactMeter")]
    public Value<int> ArrangerMixerTrackPerformanceImpactMeter { get; set; } = new() { Val = 0 };

    [XmlElement("MixerInSession")]
    public Value<int> MixerInSession { get; set; } = new() { Val = 1 };

    [XmlElement("SessionIO")]
    public Value<int> SessionIO { get; set; } = new() { Val = 1 };

    [XmlElement("SessionSends")]
    public Value<int> SessionSends { get; set; } = new() { Val = 1 };

    [XmlElement("SessionReturns")]
    public Value<int> SessionReturns { get; set; } = new() { Val = 1 };

    [XmlElement("SessionVolume")]
    public Value<int> SessionVolume { get; set; } = new() { Val = 1 };

    [XmlElement("SessionTrackOptions")]
    public Value<int> SessionTrackOptions { get; set; } = new() { Val = 0 };

    [XmlElement("SessionCrossFade")]
    public Value<int> SessionCrossFade { get; set; } = new() { Val = 0 };

    [XmlElement("SessionTrackDelay")]
    public Value<int> SessionTrackDelay { get; set; }

    [XmlElement("SessionTrackPerformanceImpactMeter")]
    public Value<int> SessionTrackPerformanceImpactMeter { get; set; } = new() { Val = 0 };

    [XmlElement("SessionShowOverView")]
    public Value<int> SessionShowOverView { get; set; } = new() { Val = 0 };

    [XmlElement("ArrangerIO")]
    public Value<int> ArrangerIO { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerReturns")]
    public Value<int> ArrangerReturns { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerVolume")]
    public Value<int> ArrangerVolume { get; set; } = new() { Val = 1 };

    [XmlElement("ArrangerTrackOptions")]
    public Value<int> ArrangerTrackOptions { get; set; } = new() { Val = 0 };

    [XmlElement("ArrangerShowOverView")]
    public Value<int> ArrangerShowOverView { get; set; }

    [XmlElement("ArrangerTrackDelay")]
    public Value<int> ArrangerTrackDelay { get; set; }

    [XmlElement("ArrangerMixer")]
    public Value<int> ArrangerMixer { get; set; }
}