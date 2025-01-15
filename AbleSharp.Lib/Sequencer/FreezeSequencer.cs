using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class FreezeSequencer
{
    [XmlElement("LomId")]
    public Value<int> LomId { get; set; }

    [XmlElement("LomIdView")]
    public Value<int> LomIdView { get; set; }

    [XmlElement("IsExpanded")]
    public Value<bool> IsExpanded { get; set; }

    [XmlElement("On")]
    public On On { get; set; }

    [XmlElement("ModulationSourceCount")]
    public Value<int> ModulationSourceCount { get; set; }

    [XmlElement("ParametersListWrapper")]
    public ParametersListWrapper ParametersListWrapper { get; set; }

    [XmlElement("Pointee")]
    public Pointee Pointee { get; set; }

    [XmlElement("LastSelectedTimeableIndex")]
    public Value<int> LastSelectedTimeableIndex { get; set; }

    [XmlElement("LastSelectedClipEnvelopeIndex")]
    public Value<int> LastSelectedClipEnvelopeIndex { get; set; }

    [XmlElement("LastPresetRef")]
    public LastPresetRef LastPresetRef { get; set; }

    [XmlElement("LockedScripts")]
    public LockedScripts LockedScripts { get; set; }

    [XmlElement("IsFolded")]
    public Value<bool> IsFolded { get; set; }

    [XmlElement("ShouldShowPresetName")]
    public Value<bool> ShouldShowPresetName { get; set; }

    [XmlElement("UserName")]
    public Value<string> UserName { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("SourceContext")]
    public SourceContext SourceContext { get; set; }

    [XmlArray("ClipSlotList")]
    [XmlArrayItem("ClipSlot")]
    public List<ClipSlot> ClipSlotList { get; set; }

    [XmlElement("MonitoringEnum")]
    public Value<MonitoringEnum> MonitoringEnum { get; set; }

    [XmlElement("KeepRecordMonitoringLatency")]
    public Value<bool> KeepRecordMonitoringLatency { get; set; }

    [XmlElement("Sample")]
    public Sample Sample { get; set; }

    [XmlElement("VolumeModulationTarget")]
    public ModulationTarget VolumeModulationTarget { get; set; }

    [XmlElement("TranspositionModulationTarget")]
    public ModulationTarget TranspositionModulationTarget { get; set; }

    [XmlElement("TransientEnvelopeModulationTarget")]
    public ModulationTarget TransientEnvelopeModulationTarget { get; set; }

    [XmlElement("GrainSizeModulationTarget")]
    public ModulationTarget GrainSizeModulationTarget { get; set; }

    [XmlElement("FluxModulationTarget")]
    public ModulationTarget FluxModulationTarget { get; set; }

    [XmlElement("SampleOffsetModulationTarget")]
    public ModulationTarget SampleOffsetModulationTarget { get; set; }

    [XmlElement("ComplexProFormantsModulationTarget")]
    public ModulationTarget ComplexProFormantsModulationTarget { get; set; }

    [XmlElement("ComplexProEnvelopeModulationTarget")]
    public ModulationTarget ComplexProEnvelopeModulationTarget { get; set; }

    [XmlElement("PitchViewScrollPosition")]
    public Value<int> PitchViewScrollPosition { get; set; } = new() { Val = -1073741824 };

    [XmlElement("SampleOffsetModulationScrollPosition")]
    public Value<int> SampleOffsetModulationScrollPosition { get; set; } = new() { Val = -1073741824 };

    [XmlElement("Recorder")]
    public Recorder Recorder { get; set; }
}