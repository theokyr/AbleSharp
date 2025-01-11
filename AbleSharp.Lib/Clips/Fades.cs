using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Fades
{
    [XmlElement("FadeInLength")]
    public Value<decimal> FadeInLength { get; set; }

    [XmlElement("FadeOutLength")]
    public Value<decimal> FadeOutLength { get; set; }

    [XmlElement("ClipFadesAreInitialized")]
    public Value<bool> ClipFadesAreInitialized { get; set; }

    [XmlElement("CrossfadeInState")]
    public Value<int> CrossfadeInState { get; set; }

    [XmlElement("FadeInCurveSkew")]
    public Value<decimal> FadeInCurveSkew { get; set; }

    [XmlElement("FadeInCurveSlope")]
    public Value<decimal> FadeInCurveSlope { get; set; }

    [XmlElement("FadeOutCurveSkew")]
    public Value<decimal> FadeOutCurveSkew { get; set; }

    [XmlElement("FadeOutCurveSlope")]
    public Value<decimal> FadeOutCurveSlope { get; set; }

    [XmlElement("IsDefaultFadeIn")]
    public Value<bool> IsDefaultFadeIn { get; set; }

    [XmlElement("IsDefaultFadeOut")]
    public Value<bool> IsDefaultFadeOut { get; set; }
}