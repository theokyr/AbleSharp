namespace AbleSharp.Lib.Schema;

/// <summary>
/// Provides a centralized view of the Ableton schema versions bundled with AbleSharp.
/// </summary>
public static partial class SchemaCatalog
{
    /// <summary>
    /// Gets the fully qualified namespace for a generated schema namespace suffix.
    /// </summary>
    /// <param name="namespaceSuffix">The namespace suffix (e.g. v12_0_12203).</param>
    public static string GetSchemaNamespace(string namespaceSuffix) => $"{SchemaNamespaceRoot}.{namespaceSuffix}";
}

/// <summary>
/// Describes a schema version that is available to the migration system.
/// </summary>
/// <param name="DisplayName">Human-readable display name.</param>
/// <param name="VersionId">Ableton version identifier (e.g. 12.0_12203).</param>
/// <param name="NamespaceSuffix">Namespace suffix used by generated classes.</param>
public sealed record SchemaVersionInfo(string DisplayName, string VersionId, string NamespaceSuffix)
{
    /// <summary>
    /// Gets the fully-qualified namespace containing generated types for the schema.
    /// </summary>
    public string Namespace => SchemaCatalog.GetSchemaNamespace(NamespaceSuffix);
}

