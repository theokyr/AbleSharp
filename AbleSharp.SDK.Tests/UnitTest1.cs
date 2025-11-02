using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AbleSharp.Lib.Schema;
using AbleSharp.SDK;
using AbleSharp.SDK.Options;
using NUnit.Framework;

namespace AbleSharp.SDK.Tests;

[TestFixture]
public class AbleSharpSdkTests
{
    [Test]
    public void AvailableVersionsIncludeCanonicalVersion()
    {
        var sdk = AbleSharpSdk.Instance;
        var canonicalId = SchemaCatalog.Canonical.VersionId;

        Assert.That(sdk.AvailableVersions.Select(v => v.MinorVersion), Does.Contain(canonicalId));
    }

    [Test]
    public void IsVersionSupported_ReturnsFalseForUnknownVersion()
    {
        var sdk = AbleSharpSdk.Instance;
        var unknownVersion = new AbletonVersion("Unsupported", "99.0_99999", "v99_0_99999");

        Assert.That(sdk.IsVersionSupported(unknownVersion), Is.False);
    }

    [Test]
    public void RegisterSchemaManifest_ThrowsWhenFileMissing()
    {
        var sdk = CreateIsolatedSdk();
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.json");

        Assert.Throws<FileNotFoundException>(() => sdk.RegisterSchemaManifest(missingPath));
    }

    [Test]
    public void CreateProject_RespectsCreationOptions()
    {
        var sdk = CreateIsolatedSdk();
        var options = new ProjectCreationOptions
        {
            TargetMinorVersion = SchemaCatalog.Canonical.VersionId,
            Tempo = 128,
            CreateDefaultTracks = false
        };

        var project = sdk.CreateProject(options);

        Assert.Multiple(() =>
        {
            Assert.That(project.MinorVersion, Is.EqualTo(options.TargetMinorVersion));
            Assert.That(project.LiveSet, Is.Not.Null);

            var liveSet = project.LiveSet!;
            Assert.That(liveSet.Tracks, Is.Not.Null);
            Assert.That(liveSet.Tracks, Is.Empty);
            Assert.That(liveSet.MainTrack, Is.Not.Null);

            var tempo = liveSet.MainTrack!.DeviceChain?.Mixer?.Tempo?.Manual?.Val;
            Assert.That(tempo, Is.Not.Null);
            Assert.That(tempo!, Is.EqualTo(options.Tempo).Within(0.01m));
        });
    }

    private static AbleSharpSdk CreateIsolatedSdk()
    {
        var constructor = typeof(AbleSharpSdk)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes);

        Assert.That(constructor, Is.Not.Null, "AbleSharpSdk no longer exposes a suitable non-public constructor for testing.");
        var sdk = (AbleSharpSdk)constructor!.Invoke(null);

        return sdk;
    }
}
