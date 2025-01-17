using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class MidiControllers
{
    private readonly Dictionary<string, ControllerTarget> _controllerTargets = new();

    [XmlAnyElement]
    public System.Xml.XmlElement[] SerializeTargets
    {
        get
        {
            var doc = new System.Xml.XmlDocument();
            var elements = new List<System.Xml.XmlElement>();

            foreach (var kvp in _controllerTargets.OrderBy(x => int.Parse(x.Key.Split('.')[1])))
            {
                var element = doc.CreateElement(kvp.Key);
                element.SetAttribute("Id", kvp.Value.Id);

                var lockElement = doc.CreateElement("LockEnvelope");
                lockElement.SetAttribute("Value", kvp.Value.LockEnvelope.Val.ToString());
                element.AppendChild(lockElement);

                elements.Add(element);
            }

            return elements.ToArray();
        }
        set
        {
            _controllerTargets.Clear();
            foreach (var element in value)
            {
                var id = element.GetAttribute("Id");
                var lockEnvValue = int.Parse(element.SelectSingleNode("LockEnvelope").Attributes["Value"].Value);

                _controllerTargets[element.Name] = new ControllerTarget
                {
                    Id = id,
                    LockEnvelope = new Value<int> { Val = lockEnvValue }
                };
            }
        }
    }
}

public class ControllerTarget
{
    public string Id { get; set; }
    public Value<int> LockEnvelope { get; set; }
}