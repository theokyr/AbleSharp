using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Mixer
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

    [XmlArray("Sends")]
    [XmlArrayItem("Send")]
    public List<Send> Sends { get; set; }

    [XmlElement("Speaker")]
    public Speaker Speaker { get; set; }

    [XmlElement("SoloSink")]
    public Value<bool> SoloSink { get; set; }

    [XmlElement("PanMode")]
    public Value<int> PanMode { get; set; }

    [XmlElement("Pan")]
    public Pan Pan { get; set; }

    [XmlElement("Volume")]
    public Volume Volume { get; set; }

    [XmlElement("Tempo")]
    public Tempo Tempo { get; set; }

    [XmlElement("ViewStateSesstionTrackWidth")]
    public Value<decimal> ViewStateSesstionTrackWidth { get; set; }

    [XmlElement("CrossFadeState")]
    public CrossFadeState CrossFadeState { get; set; }

    [XmlElement("SendsListWrapper")]
    public SendsListWrapper SendsListWrapper { get; set; }
}