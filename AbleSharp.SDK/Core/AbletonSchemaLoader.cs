using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using AbleSharp.Lib;

namespace AbleSharp.SDK;

public static class AbletonSchemaLoader
{
    public static AbletonProject LoadFromFile(string filePath, string schemaVersion)
    {
        // Always use our core AbletonProject type
        var rootType = SchemaTypeResolver.GetRootType();

        using var fileStream = File.OpenRead(filePath);
        using var memStream = new MemoryStream();
        fileStream.CopyTo(memStream);
        memStream.Position = 0;

        try
        {
            // Try to decompress as gzip
            using var gzipStream = new GZipStream(memStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Position = 0;

            var serializer = new XmlSerializer(rootType);
            return (AbletonProject)serializer.Deserialize(decompressedStream);
        }
        catch (InvalidDataException)
        {
            // If gzip decompression fails, try to deserialize directly
            memStream.Position = 0;
            var serializer = new XmlSerializer(rootType);
            return (AbletonProject)serializer.Deserialize(memStream);
        }
    }
}