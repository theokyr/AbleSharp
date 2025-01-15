using System.Xml.Serialization;

namespace AbleSharp.Lib;

public abstract class FreezableTrack : Track
{
    [XmlElement("Freeze")]
    public Value<bool> Freeze { get; set; }

    [XmlElement("NeedArrangerRefreeze")]
    public Value<bool> NeedArrangerRefreeze { get; set; }

    [XmlElement("PostProcessFreezeClips")]
    public Value<int> PostProcessFreezeClips { get; set; }
}