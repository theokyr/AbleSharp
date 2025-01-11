using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Sample
{
    [XmlElement("ArrangerAutomation")]
    public ArrangerAutomation ArrangerAutomation { get; set; }
}