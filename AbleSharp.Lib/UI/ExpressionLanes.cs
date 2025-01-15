using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ExpressionLanes
{
    [XmlElement("MidiEditorLaneModel")]
    public List<MidiEditorLaneModel> Lanes { get; set; } = new()
    {
        new() { Id = "0", Type = new() { Val = 0 }, Size = new() { Val = 41 }, IsMinimized = new() { Val = false } },
        new() { Id = "1", Type = new() { Val = 1 }, Size = new() { Val = 41 }, IsMinimized = new() { Val = false } },
        new() { Id = "2", Type = new() { Val = 2 }, Size = new() { Val = 41 }, IsMinimized = new() { Val = true } },
        new() { Id = "3", Type = new() { Val = 3 }, Size = new() { Val = 41 }, IsMinimized = new() { Val = true } }
    };
}

public class ContentLanes
{
    [XmlElement("MidiEditorLaneModel")]
    public List<MidiEditorLaneModel> Lanes { get; set; } = new()
    {
        new() { Id = "0", Type = new() { Val = 4 }, Size = new() { Val = 41 }, IsMinimized = new() { Val = false } },
        new() { Id = "1", Type = new() { Val = 5 }, Size = new() { Val = 25 }, IsMinimized = new() { Val = true } }
    };
}

public class MidiEditorLaneModel
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("Type")]
    public Value<int> Type { get; set; }

    [XmlElement("Size")]
    public Value<int> Size { get; set; }

    [XmlElement("IsMinimized")]
    public Value<bool> IsMinimized { get; set; }
}