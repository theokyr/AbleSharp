using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipTimeable
{
    [XmlElement("ArrangerAutomation")]
    public ArrangerAutomation ArrangerAutomation { get; set; }
}