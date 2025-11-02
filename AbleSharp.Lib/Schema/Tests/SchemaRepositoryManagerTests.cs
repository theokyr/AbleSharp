using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using NUnit.Framework;

namespace AbleSharp.Lib.Schema.Tests;

[TestFixture]
public class SchemaRepositoryManagerTests
{
    [Test]
    public void RegisterSchemaRoot_AddsDirectoryAndRaisesChangeEvent()
    {
        var tempRoot = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        try
        {
            using var manager = new SchemaRepositoryManager();
            using var signaled = new ManualResetEventSlim();
            SchemaRepositoryChangedEventArgs? captured = null;

            manager.RepositoryChanged += (_, args) =>
            {
                captured = args;
                signaled.Set();
            };

            manager.RegisterSchemaRoot(tempRoot.FullName);

            Assert.That(signaled.Wait(TimeSpan.FromSeconds(2)), Is.True, "RepositoryChanged was not raised for schema root registration.");
            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.Kind, Is.EqualTo(SchemaRepositoryChangeKind.SchemaRootAdded));
            Assert.That(manager.Current.SchemaRoots, Does.Contain(Path.GetFullPath(tempRoot.FullName)));
        }
        finally
        {
            tempRoot.Delete(recursive: true);
        }
    }

    [Test]
    public void RegisterManifest_MergesDescriptorsIntoRepository()
    {
        var tempRoot = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        try
        {
            var (manifestPath, schemaPath, descriptorVersion) = CreateCustomManifest(tempRoot.FullName);

            using var manager = new SchemaRepositoryManager();
            manager.RegisterSchemaRoot(tempRoot.FullName);
            manager.RegisterManifest(manifestPath);

            var descriptor = manager.Current.GetDescriptor(descriptorVersion);

            Assert.Multiple(() =>
            {
                Assert.That(descriptor, Is.Not.Null);
                Assert.That(descriptor.FullPath, Is.EqualTo(schemaPath));
            });
        }
        finally
        {
            tempRoot.Delete(recursive: true);
        }
    }

    [Test]
    public void WatchForChanges_RaisesFileChangedWhenSchemaMutates()
    {
        var tempRoot = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        try
        {
            var (manifestPath, schemaPath, _) = CreateCustomManifest(tempRoot.FullName);

            var managerOptions = new SchemaRepositoryManagerOptions
            {
                WatchForChanges = true,
                ChangeDebounceInterval = TimeSpan.FromMilliseconds(50)
            };

            using var manager = new SchemaRepositoryManager(null, managerOptions);
            using var changed = new ManualResetEventSlim();

            manager.RepositoryChanged += (_, args) =>
            {
                if (args.Kind == SchemaRepositoryChangeKind.FileChanged)
                {
                    changed.Set();
                }
            };

            manager.RegisterSchemaRoot(tempRoot.FullName);
            manager.RegisterManifest(manifestPath);

            // Ensure watcher is active before modifying.
            Thread.Sleep(100);
            File.AppendAllText(schemaPath, "\n<!-- touched -->");

            Assert.That(changed.Wait(TimeSpan.FromSeconds(3)), Is.True, "Expected file change to trigger repository reload.");
        }
        finally
        {
            tempRoot.Delete(recursive: true);
        }
    }

    private static (string ManifestPath, string SchemaPath, string VersionId) CreateCustomManifest(string rootDirectory)
    {
        var schemaPath = Path.Combine(rootDirectory, "99.0_custom.txt");
        File.WriteAllText(schemaPath, "<AbletonSchema>\n  <Sample />\n</AbletonSchema>");

        var versionId = "99.0_custom";
        var manifestPath = Path.Combine(rootDirectory, "schema-descriptors.json");
        var sha256 = ComputeSha256(schemaPath);
        var sizeBytes = new FileInfo(schemaPath).Length;

        var descriptorModel = new[]
        {
            new
            {
                versionId,
                majorVersion = 99,
                minorVersion = "0",
                schemaChangeCount = 1,
                fileName = Path.GetFileName(schemaPath),
                sizeBytes,
                sha256,
                lastModified = DateTime.UtcNow
            }
        };

        var json = JsonSerializer.Serialize(descriptorModel, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(manifestPath, json);

        return (manifestPath, schemaPath, versionId);
    }

    private static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

