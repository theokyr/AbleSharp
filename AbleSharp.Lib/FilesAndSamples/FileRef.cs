using System.Xml.Serialization;

namespace AbleSharp.Lib;

// Base FileRef class with common properties
public abstract class FileRefBase
{
    [XmlElement("RelativePathType")]
    public Value<int> RelativePathType { get; set; }

    [XmlElement("RelativePath")]
    public Value<string> RelativePath { get; set; }

    [XmlElement("Path")]
    public Value<string> Path { get; set; }

    [XmlElement("Type")]
    public Value<int> Type { get; set; }

    [XmlElement("LivePackName")]
    public Value<string> LivePackName { get; set; }

    [XmlElement("LivePackId")]
    public Value<string> LivePackId { get; set; }

    [XmlElement("OriginalFileSize")]
    public Value<long> OriginalFileSize { get; set; }

    [XmlElement("OriginalCrc")]
    public Value<int> OriginalCrc { get; set; }
}

// FileRef for SampleRef - no Id attribute
public class FileRef : FileRefBase
{
}

// FileRef for SourceContext - includes Id attribute
public class SourceContextFileRef : FileRefBase
{
    [XmlAttribute("Id")]
    public int Id { get; set; }
}