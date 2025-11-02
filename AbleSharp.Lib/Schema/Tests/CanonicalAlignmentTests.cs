using System;
using NUnit.Framework;

namespace AbleSharp.Lib.Schema.Tests;

[TestFixture]
public sealed class CanonicalAlignmentTests
{
    [Test]
    public void CanonicalDescriptor_IsRegistered()
    {
        var repository = new SchemaRepository();
        var descriptor = repository.EnsureDescriptor(SchemaCatalog.CanonicalVersionId);

        Assert.That(descriptor.VersionId, Is.EqualTo(SchemaCatalog.CanonicalVersionId));
        Assert.That(descriptor.FullPath, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void CanonicalGeneratedNamespace_IsAvailable()
    {
        var assembly = typeof(SchemaCatalog).Assembly;
        var canonicalNamespace = $"{SchemaCatalog.SchemaNamespaceRoot}.{SchemaCatalog.CanonicalNamespaceSuffix}";
        var canonicalType = assembly.GetType($"{canonicalNamespace}.AbletonDevicePreset", throwOnError: false, ignoreCase: false);

        Assert.That(canonicalType, Is.Not.Null, "Expected generated canonical schema types to be loadable.");
    }
}

