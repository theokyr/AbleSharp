using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class ClipEnvelopeChooserViewState
{
    public Value<int> SelectedDevice { get; set; }
    public Value<int> SelectedEnvelope { get; set; }
    public Value<bool> PreferModulationVisible { get; set; }
}