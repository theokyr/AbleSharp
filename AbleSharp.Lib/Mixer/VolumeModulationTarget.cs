using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class VolumeModulationTarget
{
    public Value<int> LockEnvelope { get; set; }
}