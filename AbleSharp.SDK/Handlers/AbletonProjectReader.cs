using System.IO.Compression;
using System.Xml.Serialization;
using AbleSharp.Lib;

namespace AbleSharp.SDK.Handlers;

public class AbletonProjectReader
{
    public static AbletonProject LoadFromFile(string filePath)
    {
        // Read the gzipped file into memory
        using var fileStream = File.OpenRead(filePath);
        using var memStream = new MemoryStream();
        fileStream.CopyTo(memStream);
        memStream.Position = 0;

        // Try to decompress as gzip
        try
        {
            using var gzipStream = new GZipStream(memStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Position = 0;

            // Deserialize the XML
            var serializer = new XmlSerializer(typeof(AbletonProject));
            return (AbletonProject)serializer.Deserialize(decompressedStream)!;
        }
        catch (InvalidDataException)
        {
            // If gzip decompression fails, try to deserialize directly (might be already decompressed)
            memStream.Position = 0;
            var serializer = new XmlSerializer(typeof(AbletonProject));
            return (AbletonProject)serializer.Deserialize(memStream)!;
        }
    }
}