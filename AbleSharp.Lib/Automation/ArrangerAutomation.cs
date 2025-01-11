using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ArrangerAutomation
{
    [XmlArray("Events")]
    [XmlArrayItem("AudioClip", typeof(AudioClip))]
    [XmlArrayItem("MidiClip", typeof(MidiClip))]
    public List<Clip> Events { get; set; } = [];

    [XmlElement("AutomationTransformViewState")]
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}