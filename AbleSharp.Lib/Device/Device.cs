using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Device
{
    public Value<int> Id { get; set; }
    public Value<int> LomId { get; set; }
    public Value<bool> IsExpanded { get; set; }
    public On On { get; set; }
    public Value<int> ModulationSourceCount { get; set; }
    public ParametersListWrapper ParametersListWrapper { get; set; }
    public Value<string> LastPresetRef { get; set; }
    public Value<bool> IsFolded { get; set; }
    public Value<bool> ShouldShowPresetName { get; set; }
    public Value<string> UserName { get; set; }
    public Value<string> Annotation { get; set; }
    public List<Parameter> Parameters { get; set; }
}