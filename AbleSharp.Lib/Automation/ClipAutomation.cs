using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipAutomation
{
    public List<MidiClip> Events { get; set; }
    public AutomationTransformViewState AutomationTransformViewState { get; set; }
}