using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TrackName
{
    [XmlElement("EffectiveName")]
    public Value<string> EffectiveName { get; set; }

    [XmlElement("UserName")]
    public Value<string> UserName { get; set; }

    [XmlElement("Annotation")]
    public Value<string> Annotation { get; set; }

    [XmlElement("MemorizedFirstClipName")]
    public Value<string> MemorizedFirstClipName { get; set; }
}