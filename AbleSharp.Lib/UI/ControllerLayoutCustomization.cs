using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ControllerLayoutCustomization
{
    public Value<int> PitchClassSource { get; set; }
    public Value<int> OctaveSource { get; set; }
    public Value<int> KeyNoteTarget { get; set; }
    public Value<int> StepSize { get; set; }
    public Value<int> OctaveEvery { get; set; }
    public Value<int> AllowedKeys { get; set; }
    public Value<int> FillerKeysMapTo { get; set; }
}