using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class DeviceChain
{
    [XmlElement("AutomationLanes")]
    public AutomationLanes AutomationLanes { get; set; }

    [XmlElement("ClipEnvelopeChooserViewState")]
    public ClipEnvelopeChooserViewState ClipEnvelopeChooserViewState { get; set; }

    [XmlElement("MidiInputRouting")]
    public MidiInputRouting MidiInputRouting { get; set; }

    [XmlElement("AudioInputRouting")]
    public AudioInputRouting AudioInputRouting { get; set; }

    [XmlElement("AudioOutputRouting")]
    public AudioOutputRouting AudioOutputRouting { get; set; }

    [XmlElement("MidiOutputRouting")]
    public MidiOutputRouting MidiOutputRouting { get; set; }

    [XmlElement("Mixer")]  
    public Mixer Mixer { get; set; }

    [XmlElement("MainSequencer")]
    public MainSequencer MainSequencer { get; set; }

    [XmlElement("FreezeSequencer")]
    public FreezeSequencer FreezeSequencer { get; set; }

    [XmlElement("DeviceChain")]
    public DeviceChain InnerDeviceChain { get; set; }

    [XmlArray("Devices")]
    [XmlArrayItem("Device")]
    public List<Device> Devices { get; set; } = new();

    [XmlElement("SignalModulations")]
    public SignalModulations SignalModulations { get; set; } = new();
}