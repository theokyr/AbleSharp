using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace AbleSharp.SDK;

public static class AbletonSchemaWriter
{
    public static void SaveToFile(object schemaObject, string filePath)
    {
        using var memStream = new MemoryStream();
        var serializer = new XmlSerializer(schemaObject.GetType());

        serializer.Serialize(memStream, schemaObject);
        memStream.Position = 0;

        using var fileStream = File.Create(filePath);
        using var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal);
        memStream.CopyTo(gzipStream);
    }
}