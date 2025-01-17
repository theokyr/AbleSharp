using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class SourceContext
{
    [XmlElement("Value")]
    public string Value { get; set; }

    [XmlElement("SourceContext")]
    public InnerSourceContext Inner { get; set; }
}

public class InnerSourceContext
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("OriginalFileRef")]
    public OriginalFileRef OriginalFileRef { get; set; }

    [XmlElement("BrowserContentPath")]
    public Value<string> BrowserContentPath { get; set; }

    [XmlElement("LocalFiltersJson")]
    public Value<string> LocalFiltersJson { get; set; }
}

public class OriginalFileRef
{
    [XmlElement("FileRef")]
    public SourceContextFileRef FileRef { get; set; }
}