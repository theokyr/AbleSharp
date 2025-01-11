using System.Xml.Serialization;

namespace AbleSharp.Lib;

public enum MonitoringEnum
{
    [XmlEnum("0")]
    Off = 0,

    [XmlEnum("1")]
    In = 1,

    [XmlEnum("2")]
    Auto = 2
}