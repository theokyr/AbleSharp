using System.Xml.Serialization;

namespace AbleSharp.Lib;

[XmlRoot("Ableton")]
public class AbletonProject
{
    [XmlAttribute]
    public int MajorVersion { get; set; }

    [XmlAttribute]
    public string MinorVersion { get; set; }

    [XmlAttribute]
    public int SchemaChangeCount { get; set; }

    [XmlAttribute]
    public string Creator { get; set; }

    [XmlAttribute]
    public string Revision { get; set; }

    [XmlElement("LiveSet")]
    public LiveSet LiveSet { get; set; }
}