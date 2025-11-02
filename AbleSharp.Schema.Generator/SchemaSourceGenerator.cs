using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using AbleSharp.Schema.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AbleSharp.Schema.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class SchemaSourceGenerator : IIncrementalGenerator
{
    private const string GenerationPlanFileName = "schema-generation-plan.json";

    private static readonly DiagnosticDescriptor MissingPlanDescriptor = new(
        id: "ASG001",
        title: "Schema generation plan missing",
        messageFormat: "AbleSharp schema generation plan '{0}' could not be located or parsed",
        category: "SchemaGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingSchemaDescriptor = new(
        id: "ASG002",
        title: "Schema asset missing",
        messageFormat: "Schema asset '{0}' referenced by the generation plan was not supplied as an AdditionalFile",
        category: "SchemaGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var planProvider = context.AdditionalTextsProvider
            .Where(IsGenerationPlan)
            .Select((text, cancellationToken) => text.GetText(cancellationToken)?.ToString() ?? string.Empty)
            .Select((json, _) => CreatePlan(json));

        var schemaFilesProvider = context.AdditionalTextsProvider
            .Where(IsSchemaFile)
            .Select((text, cancellationToken) => new SchemaFileSnapshot(
                Path.GetFileName(text.Path),
                text.GetText(cancellationToken)?.ToString() ?? string.Empty));

        var combined = planProvider.Combine(schemaFilesProvider.Collect());

        context.RegisterSourceOutput(combined, GenerateSources);
    }

    private static void GenerateSources(SourceProductionContext context, (SchemaGenerationPlan? Plan, ImmutableArray<SchemaFileSnapshot> Files) data)
    {
        var (plan, files) = data;

        if (plan is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(MissingPlanDescriptor, null, GenerationPlanFileName));
            return;
        }

        var fileMap = files
            .GroupBy(f => f.FileName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Last().Content, StringComparer.OrdinalIgnoreCase);

        EmitCatalog(context, plan);
        var emitter = new SchemaCodeEmitter();

        foreach (var target in plan.Targets)
        {
            if (!fileMap.TryGetValue(target.SchemaFile, out var schemaContent) || string.IsNullOrWhiteSpace(schemaContent))
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingSchemaDescriptor, null, target.SchemaFile));
                continue;
            }

            try
            {
                var source = emitter.Generate(plan.NamespaceRoot, target, schemaContent);
                var hintName = $"Schema.Generated.{target.NamespaceSuffix}.g.cs";
                context.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                var descriptor = new DiagnosticDescriptor(
                    id: "ASG003",
                    title: "Schema generation failed",
                    messageFormat: "Schema generation for '{0}' failed: {1}",
                    category: "SchemaGeneration",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);
                context.ReportDiagnostic(Diagnostic.Create(descriptor, null, target.VersionId, ex.Message));
            }
        }
    }

    private static void EmitCatalog(SourceProductionContext context, SchemaGenerationPlan plan)
    {
        var catalogSource = SchemaCatalogEmitter.Emit(plan);
        context.AddSource("SchemaCatalog.Generated.g.cs", SourceText.From(catalogSource, Encoding.UTF8));
    }

    private static bool IsGenerationPlan(AdditionalText text)
    {
        var fileName = Path.GetFileName(text.Path);
        return string.Equals(fileName, GenerationPlanFileName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSchemaFile(AdditionalText text)
    {
        var extension = Path.GetExtension(text.Path);
        return string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase);
    }

    private static SchemaGenerationPlan? CreatePlan(string json)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return SchemaGenerationPlan.Load(json);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private sealed record SchemaFileSnapshot(string FileName, string Content);
}

