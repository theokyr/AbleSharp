using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MainSequencer
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

    [XmlElement("ClipTimeable")]
    public ClipTimeable ClipTimeable { get; set; }

    [XmlElement("Sample")]
    public Sample Sample { get; set; }

    [XmlElement("Recorder")]
    public Recorder Recorder { get; set; }

    [XmlElement("MidiControllers")]
    public MidiControllers MidiControllers { get; set; }
}