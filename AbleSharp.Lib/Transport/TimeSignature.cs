using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class TimeSignature
{
    [XmlArray("TimeSignatures")]
    [XmlArrayItem("RemoteableTimeSignature")]
    public List<RemoteableTimeSignature> TimeSignatures { get; set; }
}