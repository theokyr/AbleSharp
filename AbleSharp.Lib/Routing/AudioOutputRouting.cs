using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class AudioOutputRouting : Routing
{
    public AudioOutputRouting()
    {
        Target = new Value<string> { Val = "AudioOut/Main" };
        UpperDisplayString = new Value<string> { Val = "Master" };
        LowerDisplayString = new Value<string> { Val = "" };
        MpeSettings = new MpeSettings();
    }
}