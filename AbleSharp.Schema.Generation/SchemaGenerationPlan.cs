using System.Collections.Immutable;
using System.Text.Json;

namespace AbleSharp.Schema.Generation;

/// <summary>
/// Represents the schema generation plan consumed by the source generator and build tooling.
/// </summary>
public sealed class SchemaGenerationPlan
{
    private readonly ImmutableDictionary<string, SchemaGenerationTarget> _targets;

    private SchemaGenerationPlan(
        string namespaceRoot,
        string canonicalVersionId,
        int canonicalSchemaChangeCount,
        ImmutableArray<SchemaGenerationTarget> targets)
    {
        NamespaceRoot = namespaceRoot;
        CanonicalVersionId = canonicalVersionId;
        CanonicalSchemaChangeCount = canonicalSchemaChangeCount;
        Targets = targets;
        _targets = targets.ToImmutableDictionary(t => t.VersionId, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the namespace root used when emitting generated schema types.
    /// </summary>
    public string NamespaceRoot { get; }

    /// <summary>
    /// Gets the canonical schema version identifier tracked by the plan.
    /// </summary>
    public string CanonicalVersionId { get; }

    /// <summary>
    /// Gets the schema change counter associated with the canonical version.
    /// </summary>
    public int CanonicalSchemaChangeCount { get; }

    /// <summary>
    /// Gets the ordered collection of schema generation targets.
    /// </summary>
    public ImmutableArray<SchemaGenerationTarget> Targets { get; }

    /// <summary>
    /// Gets the canonical schema generation target.
    /// </summary>
    public SchemaGenerationTarget CanonicalTarget => GetTarget(CanonicalVersionId);

    /// <summary>
    /// Retrieves a generation target by version identifier.
    /// </summary>
    public SchemaGenerationTarget GetTarget(string versionId)
    {
        if (!_targets.TryGetValue(versionId, out var target))
        {
            throw new KeyNotFoundException($"Schema generation plan does not contain version '{versionId}'.");
        }

        return target;
    }

    /// <summary>
    /// Loads a generation plan from a JSON payload.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown when the JSON payload is invalid.</exception>
    public static SchemaGenerationPlan Load(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidDataException("Schema generation plan JSON was empty.");
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var namespaceRoot = root.GetPropertyOrThrow("namespaceRoot").GetStringOrThrow("namespaceRoot");
        var canonicalVersionId = root.GetPropertyOrThrow("canonicalVersionId").GetStringOrThrow("canonicalVersionId");
        var canonicalChangeCount = root.GetPropertyOrThrow("canonicalSchemaChangeCount").GetInt32OrThrow("canonicalSchemaChangeCount");

        var versionsElement = root.GetPropertyOrThrow("versions");
        if (versionsElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("Schema generation plan 'versions' must be an array.");
        }

        var targets = ImmutableArray.CreateBuilder<SchemaGenerationTarget>();
        var knownVersions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in versionsElement.EnumerateArray())
        {
            var displayName = element.GetPropertyOrThrow("displayName").GetStringOrThrow("displayName");
            var versionId = element.GetPropertyOrThrow("versionId").GetStringOrThrow("versionId");
            var namespaceSuffix = element.GetPropertyOrThrow("namespaceSuffix").GetStringOrThrow("namespaceSuffix");
            var schemaFile = element.GetPropertyOrThrow("schemaFile").GetStringOrThrow("schemaFile");

            if (!knownVersions.Add(versionId))
            {
                throw new InvalidDataException($"Duplicate schema generation target detected for version '{versionId}'.");
            }

            targets.Add(new SchemaGenerationTarget(displayName, versionId, namespaceSuffix, schemaFile));
        }

        if (!knownVersions.Contains(canonicalVersionId))
        {
            throw new InvalidDataException($"Canonical version '{canonicalVersionId}' was not present in the generation plan versions list.");
        }

        return new SchemaGenerationPlan(namespaceRoot, canonicalVersionId, canonicalChangeCount, targets.ToImmutable());
    }
}

/// <summary>
/// Describes a schema generation target version.
/// </summary>
/// <param name="DisplayName">Human-friendly display label.</param>
/// <param name="VersionId">Ableton schema version identifier.</param>
/// <param name="NamespaceSuffix">Suffix appended to the namespace root when emitting code.</param>
/// <param name="SchemaFile">Schema asset file name associated with the version.</param>
public sealed record SchemaGenerationTarget(
    string DisplayName,
    string VersionId,
    string NamespaceSuffix,
    string SchemaFile)
{
    /// <summary>
    /// Gets the fully-qualified namespace for the generated schema types.
    /// </summary>
    public string GetNamespace(string namespaceRoot) => $"{namespaceRoot}.{NamespaceSuffix}";
}

internal static class JsonElementExtensions
{
    public static JsonElement GetPropertyOrThrow(this JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property))
        {
            throw new InvalidDataException($"Schema generation plan is missing required property '{name}'.");
        }

        return property;
    }

    public static string GetStringOrThrow(this JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException($"Schema generation plan property '{propertyName}' must be a string.");
        }

        var value = element.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"Schema generation plan property '{propertyName}' cannot be empty.");
        }

        return value!;
    }

    public static int GetInt32OrThrow(this JsonElement element, string propertyName)
    {
        if (!element.TryGetInt32(out var value))
        {
            throw new InvalidDataException($"Schema generation plan property '{propertyName}' must be an integer.");
        }

        return value;
    }
}

