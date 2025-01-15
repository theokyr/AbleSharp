using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AudioOutputRouting : Routing
{
    public AudioOutputRouting()
    {
        Target = new() { Val = "AudioOut/Main" };
        UpperDisplayString = new() { Val = "Master" };
        LowerDisplayString = new() { Val = "" };
        MpeSettings = new MpeSettings();
    }
}