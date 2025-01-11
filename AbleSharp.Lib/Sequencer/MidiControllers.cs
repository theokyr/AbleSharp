using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiControllers
{
    [XmlAnyElement]
    public List<System.Xml.XmlElement> UnknownControllerTargets { get; set; }
}