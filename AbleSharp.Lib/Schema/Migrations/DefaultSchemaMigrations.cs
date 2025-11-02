using System.Collections.Immutable;
using System.Xml.Linq;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Provides the default migration set bundled with AbleSharp.
/// </summary>
public static class DefaultSchemaMigrations
{
    private static readonly ImmutableArray<ISchemaMigration> Migrations =
    [
        HeaderOnlyMigration.Forward(
            "11.0_11202",
            "12.0_12049",
            "Upgrade Live 11.0 projects to the Live 12.0 schema."),

        HeaderOnlyMigration.Forward(
            "12.0_12049",
            "12.0_12120",
            "Upgrade Live 12.0 projects to the Live 12.1 schema."),

        HeaderOnlyMigration.Forward(
            "12.0_12120",
            SchemaCatalog.CanonicalVersionId,
            "Upgrade Live 12.1 projects to the canonical AbleSharp schema.",
            header => header.SetAttributeValue("SchemaChangeCount", SchemaCatalog.CanonicalSchemaChangeCount)),

        HeaderOnlyMigration.Backward(
            SchemaCatalog.CanonicalVersionId,
            "12.0_12120",
            "Downgrade canonical projects to Live 12.1 metadata."),

        HeaderOnlyMigration.Backward(
            "12.0_12120",
            "12.0_12049",
            "Downgrade Live 12.1 projects to the Live 12.0 schema."),

        HeaderOnlyMigration.Backward(
            "12.0_12049",
            "11.0_11202",
            "Downgrade Live 12.0 projects to the Live 11 schema.")
    ];

    /// <summary>
    /// Gets the default set of migrations in registration order.
    /// </summary>
    public static IReadOnlyList<ISchemaMigration> All => Migrations;
}

