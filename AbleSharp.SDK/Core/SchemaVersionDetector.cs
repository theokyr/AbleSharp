using System.Xml.Linq;
using System.IO;
using System.IO.Compression;

namespace AbleSharp.SDK;

public static class SchemaVersionDetector
{
    public static string DetectFromFile(string filePath)
    {
        // Read the gzipped file into memory
        using var fileStream = File.OpenRead(filePath);
        using var memStream = new MemoryStream();
        fileStream.CopyTo(memStream);
        memStream.Position = 0;

        try
        {
            string xmlContent;

            // Try to decompress as gzip
            using (var gzipStream = new GZipStream(memStream, CompressionMode.Decompress))
            using (var decompressedStream = new MemoryStream())
            {
                gzipStream.CopyTo(decompressedStream);
                decompressedStream.Position = 0;
                using var reader = new StreamReader(decompressedStream);
                xmlContent = reader.ReadToEnd();
            }

            // Parse XML to get version
            var doc = XDocument.Parse(xmlContent);
            var root = doc.Root;
            if (root?.Name.LocalName == "Ableton")
            {
                var minorVersion = root.Attribute("MinorVersion")?.Value;
                if (!string.IsNullOrEmpty(minorVersion)) return minorVersion;
            }

            throw new Exception("Could not find MinorVersion attribute in Ableton project file");
        }
        catch (InvalidDataException)
        {
            // If gzip decompression fails, try to read directly
            memStream.Position = 0;
            using var reader = new StreamReader(memStream);
            var xmlContent = reader.ReadToEnd();

            var doc = XDocument.Parse(xmlContent);
            var root = doc.Root;
            if (root?.Name.LocalName == "Ableton")
            {
                var minorVersion = root.Attribute("MinorVersion")?.Value;
                if (!string.IsNullOrEmpty(minorVersion)) return minorVersion;
            }

            throw new Exception("Could not find MinorVersion attribute in Ableton project file");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error detecting schema version: {ex.Message}", ex);
        }
    }
}