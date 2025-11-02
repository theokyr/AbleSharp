using AbleSharp.Schema.Generation;
using NUnit.Framework;

namespace AbleSharp.Schema.Generation.Tests;

[TestFixture]
public class SchemaGenerationEmitterTests
{
    [Test]
    public void SchemaCodeEmitter_Generate_ProducesVersionedNamespaceAndStubTypes()
    {
        const string namespaceRoot = "AbleSharp.Lib.Schema.Generated";
        var target = new SchemaGenerationTarget("Live 99", "99.0_custom", "v99_0_custom", "99.0_custom.txt");

        var schema = """
<AbletonSchema>
  <AbletonDevicePreset>
    <Bar Class="Slot&lt;Float&gt;" />
    <Name Class="String" />
  </AbletonDevicePreset>
  <PythonListWrapper />
</AbletonSchema>
""";

        var emitter = new SchemaCodeEmitter();
        var source = emitter.Generate(namespaceRoot, target, schema);

        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("namespace AbleSharp.Lib.Schema.Generated.v99_0_custom;"));
            Assert.That(source, Does.Contain("public class PythonListWrapper"));
            Assert.That(source, Does.Contain("public Slot<float> Bar"));
        });
    }

    [Test]
    public void SchemaCatalogEmitter_Emit_ContainsCanonicalMetadata()
    {
        var planJson = """
{
  "namespaceRoot": "AbleSharp.Lib.Schema.Generated",
  "canonicalVersionId": "12.0_12203",
  "canonicalSchemaChangeCount": 12203,
  "versions": [
    {
      "displayName": "Live 11",
      "versionId": "11.0_11202",
      "namespaceSuffix": "v11_0_11202",
      "schemaFile": "11.0_11202.txt"
    },
    {
      "displayName": "Live 12",
      "versionId": "12.0_12203",
      "namespaceSuffix": "v12_0_12203",
      "schemaFile": "12.0_12203.txt"
    }
  ]
}
""";

        var plan = SchemaGenerationPlan.Load(planJson);
        var source = SchemaCatalogEmitter.Emit(plan);

        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("public const string CanonicalVersionId = \"12.0_12203\";"));
            Assert.That(source, Does.Contain("public const int CanonicalSchemaChangeCount = 12203;"));
            Assert.That(source, Does.Contain("public static SchemaVersionInfo Canonical => GeneratedVersions[^1];"));
        });
    }
}
