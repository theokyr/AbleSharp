using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Onsets
{
    public List<UserOnset> UserOnsets { get; set; }
    public Value<bool> HasUserOnsets { get; set; }
}