using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ExpressionLanes
{
    [XmlElement("MidiEditorLaneModel")]
    public List<MidiEditorLaneModel> Lanes { get; set; } = new()
    {
        new MidiEditorLaneModel { Id = "0", Type = new Value<int> { Val = 0 }, Size = new Value<int> { Val = 41 }, IsMinimized = new Value<bool> { Val = false } },
        new MidiEditorLaneModel { Id = "1", Type = new Value<int> { Val = 1 }, Size = new Value<int> { Val = 41 }, IsMinimized = new Value<bool> { Val = false } },
        new MidiEditorLaneModel { Id = "2", Type = new Value<int> { Val = 2 }, Size = new Value<int> { Val = 41 }, IsMinimized = new Value<bool> { Val = true } },
        new MidiEditorLaneModel { Id = "3", Type = new Value<int> { Val = 3 }, Size = new Value<int> { Val = 41 }, IsMinimized = new Value<bool> { Val = true } }
    };
}

public class ContentLanes
{
    [XmlElement("MidiEditorLaneModel")]
    public List<MidiEditorLaneModel> Lanes { get; set; } = new()
    {
        new MidiEditorLaneModel { Id = "0", Type = new Value<int> { Val = 4 }, Size = new Value<int> { Val = 41 }, IsMinimized = new Value<bool> { Val = false } },
        new MidiEditorLaneModel { Id = "1", Type = new Value<int> { Val = 5 }, Size = new Value<int> { Val = 25 }, IsMinimized = new Value<bool> { Val = true } }
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