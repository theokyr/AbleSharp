using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiInputRouting : Routing
{
    public MidiInputRouting()
    {
        Target = new Value<string> { Val = "MidiIn/External.All/-1" };
        UpperDisplayString = new Value<string> { Val = "Ext: All Ins" };
        LowerDisplayString = new Value<string> { Val = "" };
        MpeSettings = new MpeSettings();
    }
}