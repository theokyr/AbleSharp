using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class NoteIdGenerator
{
    public Value<int> NextId { get; set; }
}