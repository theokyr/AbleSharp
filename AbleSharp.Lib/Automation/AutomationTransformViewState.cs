using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AutomationTransformViewState
{
    [XmlElement("IsTransformPending")]
    public Value<bool> IsTransformPending { get; set; }

    [XmlElement("TimeAndValueTransforms")]
    public TimeAndValueTransforms TimeAndValueTransforms { get; set; }
}