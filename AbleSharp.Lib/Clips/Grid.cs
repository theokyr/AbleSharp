using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Grid
{
    public Value<int> FixedNumerator { get; set; }
    public Value<int> FixedDenominator { get; set; }
    public Value<decimal> GridIntervalPixel { get; set; }
    public Value<int> Ntoles { get; set; }
    public Value<bool> SnapToGrid { get; set; }
    public Value<bool> Fixed { get; set; }
}