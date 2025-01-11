using System.Xml.Serialization;

namespace AbleSharp.Lib
{
    [XmlType("GroupTrack")]
    public class GroupTrack : FreezableTrack
    {
        /// <summary>
        /// The Slots list holds one or more GroupTrackSlot elements.
        /// </summary>
        [XmlArray("Slots")]
        [XmlArrayItem("GroupTrackSlot", typeof(GroupTrackSlot))]
        public List<GroupTrackSlot> Slots { get; set; } = new List<GroupTrackSlot>();
    }
}