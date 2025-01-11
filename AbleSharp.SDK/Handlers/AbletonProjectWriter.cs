using System.IO.Compression;
using System.Xml.Serialization;
using AbleSharp.Lib;

namespace AbleSharp.SDK.Handlers;

public class AbletonProjectWriter
{
    public static void SaveToFile(AbletonProject abletonProject, string filePath)
    {
        using var memStream = new MemoryStream();
        var serializer = new XmlSerializer(typeof(AbletonProject));

        serializer.Serialize(memStream, abletonProject);
        memStream.Position = 0;

        using var fileStream = File.Create(filePath);
        using var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal);
        memStream.CopyTo(gzipStream);
    }
}