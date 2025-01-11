using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class PerNoteEventStore
{
    public List<EventList> EventLists { get; set; }
}