using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NUnit.Framework;

namespace AbleSharp.Lib.Schema.Tests;

[TestFixture]
public class SchemaRepositoryTests
{
    [Test]
    public void SchemaRepository_LoadsManifest_ProvidesDescriptors()
    {
        var repository = new SchemaRepository();

        var descriptors = repository.GetDescriptors();

        Assert.That(descriptors, Is.Not.Null);
        Assert.That(descriptors.Count, Is.GreaterThan(0), "Expected schema descriptors to be loaded from manifest.");
    }

    [Test]
    public void SchemaRepository_ValidateDescriptor_ReturnsValidForBundledSchema()
    {
        var repository = new SchemaRepository();
        var descriptor = repository.GetDescriptors().First();

        var result = repository.ValidateDescriptor(descriptor);

        Assert.That(result.IsValid, Is.True, () => $"Expected descriptor {descriptor.VersionId} to validate successfully, but received {result.Status}.");
    }

    [Test]
    public void SchemaVersionDetector_DetectsVersionAndResolvesDescriptor()
    {
        var repository = new SchemaRepository();
        var detector = new SchemaVersionDetector(repository);

        var xmlHeader = "<Ableton MajorVersion=\"12\" MinorVersion=\"0\" SchemaChangeCount=\"12203\" />";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlHeader));

        var resolution = detector.DetectVersion(stream, requireDescriptor: true);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.MajorVersion, Is.EqualTo(12));
            Assert.That(resolution.SchemaChangeCount, Is.EqualTo(12203));
            Assert.That(resolution.VersionId, Is.EqualTo("12.0_12203"));
            Assert.That(resolution.Descriptor, Is.Not.Null);
        });
    }

    [Test]
    public void SchemaVersionDetector_UnknownVersionThrowsWhenRequired()
    {
        var repository = new SchemaRepository();
        var detector = new SchemaVersionDetector(repository);

        var xmlHeader = "<Ableton MajorVersion=\"99\" MinorVersion=\"0\" SchemaChangeCount=\"1\" />";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlHeader));

        Assert.Throws<KeyNotFoundException>(() => detector.DetectVersion(stream, requireDescriptor: true));
    }

    [Test]
    public void SchemaRepository_MergesAdditionalManifestDescriptors()
    {
        var tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        try
        {
            var schemaContent = "<AbletonSchema>\n  <Sample />\n</AbletonSchema>";
            var schemaFile = Path.Combine(tempDirectory.FullName, "99.0_custom.txt");
            File.WriteAllText(schemaFile, schemaContent);

            var sizeBytes = new FileInfo(schemaFile).Length;
            var sha256 = ComputeSha256(schemaFile);

            var manifestPath = Path.Combine(tempDirectory.FullName, "schema-descriptors.json");
            var descriptorModel = new[]
            {
                new
                {
                    versionId = "99.0_custom",
                    majorVersion = 99,
                    minorVersion = "0",
                    schemaChangeCount = 1,
                    fileName = Path.GetFileName(schemaFile),
                    sizeBytes,
                    sha256,
                    lastModified = DateTime.UtcNow
                }
            };

            var descriptorJson = JsonSerializer.Serialize(descriptorModel, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(manifestPath, descriptorJson);

            var options = new SchemaRepositoryOptions();
            options.AdditionalManifestPaths.Add(manifestPath);
            options.SchemaRoots.Add(tempDirectory.FullName);

            var repository = new SchemaRepository(options);
            var descriptor = repository.GetDescriptor("99.0_custom");

            Assert.Multiple(() =>
            {
                Assert.That(descriptor, Is.Not.Null);
                Assert.That(descriptor.FullPath, Is.EqualTo(schemaFile));
            });
        }
        finally
        {
            Directory.Delete(tempDirectory.FullName, recursive: true);
        }
    }

    private static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

