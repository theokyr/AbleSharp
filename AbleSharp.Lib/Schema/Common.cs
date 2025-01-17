using System.Xml.Serialization;

namespace AbleSharp.Lib.Schema.Common;

// Base interface for remoteable values
public interface IRemoteable<T>
{
    T Value { get; set; }
}

// Basic remoteable value wrapper
public class RemoteableValue<T> : IRemoteable<T>
{
    public T Value { get; set; }
}

/// <summary>
/// Represents an Ableton RemoteablePoint element
/// </summary>
[XmlRoot("RemoteablePoint")]
public class RemoteablePoint
{
    /// <summary>
    /// Gets or sets the X
    /// </summary>
    [XmlElement("X")]
    public int XProperty { get; set; }

    /// <summary>
    /// Gets or sets the Y
    /// </summary>
    [XmlElement("Y")]
    public int YProperty { get; set; }
}

// Specialized for slots that can be automated
public class RemoteableSlot<T> : RemoteableValue<T>
{
    public AutomationTarget AutomationTarget { get; set; }
    public ModulationTarget ModulationTarget { get; set; }
}

// List of remoteable values
public class RemoteableList<T> where T : IRemoteable<object>
{
    public List<T> Items { get; set; } = new();
}

// Base class for all device routing
public interface IRoutable
{
    string Target { get; set; }
    string UpperDisplayString { get; set; }
    string LowerDisplayString { get; set; }
}

// Base class for devices
public abstract class TrackDevice
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsExpanded { get; set; }
    public DeviceChain DeviceChain { get; set; }
}

// Container for zone settings
public class ZoneRange
{
    public int Min { get; set; }
    public int Max { get; set; }
    public bool Enabled { get; set; }
}

// Zone configuration
public class ZoneSettings
{
    public List<ZoneRange> Ranges { get; set; } = new();
    public bool CrossfadeEnabled { get; set; }
}

// Grid for beats/bars
public class BeatGrid
{
    public int Numerator { get; set; }
    public int Denominator { get; set; }
    public bool SnapToGrid { get; set; }
    public int Division { get; set; }
}