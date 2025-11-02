using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AbleSharp.Schema.Generator;
using AbleSharp.Schema.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace AbleSharp.Schema.Generator.Tests;

[TestFixture]
public class SchemaSourceGeneratorTests
{
    private static readonly Type GeneratorType = typeof(SchemaSourceGenerator);
    private static readonly BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

    [Test]
    public void MissingPlanDescriptor_HasExpectedMetadata()
    {
        var descriptor = GetPrivateField<DiagnosticDescriptor>("MissingPlanDescriptor");

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.Id, Is.EqualTo("ASG001"));
            Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error));
            Assert.That(descriptor.MessageFormat.ToString(), Does.Contain("plan"));
        });
    }

    [Test]
    public void MissingSchemaDescriptor_HasExpectedMetadata()
    {
        var descriptor = GetPrivateField<DiagnosticDescriptor>("MissingSchemaDescriptor");

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.Id, Is.EqualTo("ASG002"));
            Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error));
            Assert.That(descriptor.MessageFormat.ToString(), Does.Contain("Schema asset"));
        });
    }

    [Test]
    public void CreatePlan_WithValidJson_ProducesPlan()
    {
        var json = """
{
  "namespaceRoot": "AbleSharp.Lib.Schema.Generated",
  "canonicalVersionId": "12.0_12203",
  "canonicalSchemaChangeCount": 12203,
  "versions": [
    {
      "displayName": "Live 12",
      "versionId": "12.0_12203",
      "namespaceSuffix": "v12_0_12203",
      "schemaFile": "12.0_12203.txt"
    }
  ]
}
""";

        var plan = InvokePrivateMethod<SchemaGenerationPlan?>("CreatePlan", json);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.CanonicalVersionId, Is.EqualTo("12.0_12203"));
        Assert.That(plan.Targets.Length, Is.EqualTo(1));
    }

    [Test]
    public void CreatePlan_WithInvalidJson_ReturnsNull()
    {
        var plan = InvokePrivateMethod<SchemaGenerationPlan?>("CreatePlan", "{ not json }");
        Assert.That(plan, Is.Null);
    }

    [Test]
    public void IsGenerationPlan_DetectsGenerationPlanFile()
    {
        var planText = new InMemoryAdditionalText(Path.Combine(Path.GetTempPath(), "schema-generation-plan.json"), "{}");
        var otherText = new InMemoryAdditionalText(Path.Combine(Path.GetTempPath(), "12.0_12203.txt"), "");

        var isPlan = InvokePrivateMethod<bool>("IsGenerationPlan", planText);
        var isPlanForOther = InvokePrivateMethod<bool>("IsGenerationPlan", otherText);

        Assert.Multiple(() =>
        {
            Assert.That(isPlan, Is.True);
            Assert.That(isPlanForOther, Is.False);
        });
    }

    [Test]
    public void IsSchemaFile_DetectsTxtAssets()
    {
        var schemaText = new InMemoryAdditionalText(Path.Combine(Path.GetTempPath(), "11.0_11202.txt"), "");
        var nonSchema = new InMemoryAdditionalText(Path.Combine(Path.GetTempPath(), "readme.md"), "");

        var resultSchema = InvokePrivateMethod<bool>("IsSchemaFile", schemaText);
        var resultOther = InvokePrivateMethod<bool>("IsSchemaFile", nonSchema);

        Assert.Multiple(() =>
        {
            Assert.That(resultSchema, Is.True);
            Assert.That(resultOther, Is.False);
        });
    }

    private static T GetPrivateField<T>(string name)
    {
        var field = GeneratorType.GetField(name, PrivateStatic);
        Assert.That(field, Is.Not.Null, $"Field '{name}' was not found on SchemaSourceGenerator.");
        return (T)field!.GetValue(null)!;
    }

    private static T InvokePrivateMethod<T>(string name, params object[] arguments)
    {
        var method = GeneratorType.GetMethods(PrivateStatic).FirstOrDefault(m => m.Name == name && m.GetParameters().Length == arguments.Length);
        Assert.That(method, Is.Not.Null, $"Method '{name}' was not found on SchemaSourceGenerator.");
        return (T)method!.Invoke(null, arguments)!;
    }

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly string _content;

        public InMemoryAdditionalText(string path, string content)
        {
            Path = path;
            _content = content;
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return SourceText.From(_content, Encoding.UTF8);
        }
    }
}
