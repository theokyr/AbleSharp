using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ViewStates
{
    [XmlElement("MixerInArrangement")]
    public Value<int> MixerInArrangement { get; set; }

    [XmlElement("ArrangerMixerIO")]
    public Value<int> ArrangerMixerIO { get; set; }

    [XmlElement("ArrangerMixerSends")]
    public Value<int> ArrangerMixerSends { get; set; }

    [XmlElement("ArrangerMixerReturns")]
    public Value<int> ArrangerMixerReturns { get; set; }

    [XmlElement("ArrangerMixerVolume")]
    public Value<int> ArrangerMixerVolume { get; set; }

    [XmlElement("ArrangerMixerTrackOptions")]
    public Value<int> ArrangerMixerTrackOptions { get; set; }

    [XmlElement("ArrangerMixerCrossFade")]
    public Value<int> ArrangerMixerCrossFade { get; set; }

    [XmlElement("ArrangerMixerTrackPerformanceImpactMeter")]
    public Value<int> ArrangerMixerTrackPerformanceImpactMeter { get; set; }

    [XmlElement("MixerInSession")]
    public Value<int> MixerInSession { get; set; }

    [XmlElement("SessionIO")]
    public Value<int> SessionIO { get; set; }

    [XmlElement("SessionSends")]
    public Value<int> SessionSends { get; set; }

    [XmlElement("SessionReturns")]
    public Value<int> SessionReturns { get; set; }

    [XmlElement("SessionVolume")]
    public Value<int> SessionVolume { get; set; }

    [XmlElement("SessionTrackOptions")]
    public Value<int> SessionTrackOptions { get; set; }

    [XmlElement("SessionCrossFade")]
    public Value<int> SessionCrossFade { get; set; }

    [XmlElement("SessionTrackDelay")]
    public Value<int> SessionTrackDelay { get; set; }

    [XmlElement("SessionTrackPerformanceImpactMeter")]
    public Value<int> SessionTrackPerformanceImpactMeter { get; set; }

    [XmlElement("SessionShowOverView")]
    public Value<int> SessionShowOverView { get; set; }

    [XmlElement("ArrangerIO")]
    public Value<int> ArrangerIO { get; set; }

    [XmlElement("ArrangerReturns")]
    public Value<int> ArrangerReturns { get; set; }

    [XmlElement("ArrangerVolume")]
    public Value<int> ArrangerVolume { get; set; }

    [XmlElement("ArrangerTrackOptions")]
    public Value<int> ArrangerTrackOptions { get; set; }

    [XmlElement("ArrangerShowOverView")]
    public Value<int> ArrangerShowOverView { get; set; }

    [XmlElement("ArrangerTrackDelay")]
    public Value<int> ArrangerTrackDelay { get; set; }

    [XmlElement("ArrangerMixer")]
    public Value<int> ArrangerMixer { get; set; }
}