using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SequencerNavigator
{
    public BeatTimeHelper BeatTimeHelper { get; set; }
    public ScrollerPos ScrollerPos { get; set; }
    public ClientSize ClientSize { get; set; }
}