using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class DeviceChain
{
    [XmlElement("AutomationLanes")]
    public AutomationLanes AutomationLanes { get; set; }

    [XmlElement("ClipEnvelopeChooserViewState")]
    public ClipEnvelopeChooserViewState ClipEnvelopeChooserViewState { get; set; }

    [XmlElement("AudioInputRouting")]
    public AudioInputRouting AudioInputRouting { get; set; }

    [XmlElement("AudioOutputRouting")]
    public AudioOutputRouting AudioOutputRouting { get; set; }

    [XmlElement("MidiInputRouting")]
    public MidiInputRouting MidiInputRouting { get; set; }

    [XmlElement("MidiOutputRouting")]
    public MidiOutputRouting MidiOutputRouting { get; set; }

    [XmlElement("Mixer")]
    public Mixer Mixer { get; set; }

    [XmlArray("Devices")]
    [XmlArrayItem("Device")]
    public List<Device> Devices { get; set; }

    [XmlElement("SignalModulations")]
    public SignalModulations SignalModulations { get; set; }

    private MainSequencer _mainSequencer;
    private FreezeSequencer _freezeSequencer;

    [XmlElement("MainSequencer")]
    public MainSequencer MainSequencer
    {
        get => _mainSequencer;
        set => _mainSequencer = value;
    }

    [XmlElement("FreezeSequencer")]
    public FreezeSequencer FreezeSequencer
    {
        get => _freezeSequencer;
        set => _freezeSequencer = value;
    }

    public bool ShouldSerializeMainSequencer() => MainSequencer != null;

    public bool ShouldSerializeFreezeSequencer() => FreezeSequencer != null;
}