using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

namespace Ableton.SDK.Handlers
{
    public static class SchemaAwareProjectReader
    {
        /// <summary>
        /// Loads an Ableton project from a .als or .xml file, detects "MinorVersion",
        /// and deserializes using the correct generated schema namespace.
        /// </summary>
        public static object LoadFromFile(string filePath)
        {
            // 1) Read (possible) gzipped file into memory
            using var fileStream = File.OpenRead(filePath);
            using var memStream = new MemoryStream();
            fileStream.CopyTo(memStream);
            memStream.Position = 0;

            Stream xmlStream = memStream;
            try
            {
                // Attempt to treat as GZip
                memStream.Position = 0;
                xmlStream = new GZipStream(memStream, CompressionMode.Decompress);
            }
            catch (InvalidDataException)
            {
                // If it's not gzipped, revert to raw
                memStream.Position = 0;
                xmlStream = memStream;
            }

            // 2) We first parse the root <Ableton> tag's MinorVersion attribute
            //    We'll do a "light parse" via XmlReader.
            var minorVersion = DetectMinorVersion(xmlStream);
            if (string.IsNullOrEmpty(minorVersion))
            {
                throw new InvalidOperationException(
                    "Could not find MinorVersion attribute in the <Ableton> root element."
                );
            }

            // 3) Convert MinorVersion => namespace string. e.g. "12.0_12049" => "Ableton.Lib.Schema.v12_0_12049"
            var sanitizedVersion = "v" + minorVersion.Replace('.', '_');
            var schemaNamespace = $"Ableton.Lib.Schema.{sanitizedVersion}";

            // 4) Build the fully qualified type name for the root class,
            //    e.g. "Ableton.Lib.Schema.v12_0_12049.AbletonProject".
            //    We assume the root is called "AbletonProject" in every schema version.
            var typeName = $"{schemaNamespace}.AbletonProject";

            // 5) Reflect to find that Type object
            //    (It must be loaded in the current AppDomain from your generated code!)
            var rootType = Type.GetType(typeName, throwOnError: false);
            if (rootType == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find type \"{typeName}\". Ensure the assembly containing that schema version is loaded."
                );
            }

            // 6) Reset the xmlStream for actual deserialization
            xmlStream.Position = 0; // Rewind

            // 7) Deserialize
            var serializer = new XmlSerializer(rootType);
            var deserialized = serializer.Deserialize(xmlStream);
            if (deserialized == null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize {filePath} as {typeName}."
                );
            }

            return deserialized;
        }

        /// <summary>
        /// Quickly parse the root element's MinorVersion="..." attribute from the stream.
        /// </summary>
        private static string? DetectMinorVersion(Stream stream)
        {
            // We'll do a minimal parse with XmlReader
            stream.Position = 0;
            using var reader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = false });
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Ableton")
                {
                    // Look for the MinorVersion attribute
                    var minorVer = reader.GetAttribute("MinorVersion");
                    return minorVer;
                }
            }
            return null;
        }
    }
}
