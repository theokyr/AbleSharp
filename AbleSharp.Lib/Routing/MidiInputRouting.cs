using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiInputRouting : Routing
{
    public MidiInputRouting()
    {
        Target = new() { Val = "MidiIn/External.All/-1" };
        UpperDisplayString = new() { Val = "Ext: All Ins" };
        LowerDisplayString = new() { Val = "" };
        MpeSettings = new MpeSettings();
    }
}