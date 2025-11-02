using System.Xml.Linq;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Represents an executable migration step between two schema versions.
/// </summary>
public interface ISchemaMigration
{
    /// <summary>
    /// Gets metadata describing the migration.
    /// </summary>
    SchemaMigrationMetadata Metadata { get; }

    /// <summary>
    /// Applies the migration to the provided document.
    /// </summary>
    /// <param name="document">Ableton project XML document.</param>
    /// <param name="context">Execution context containing shared services.</param>
    /// <returns>The migrated document.</returns>
    XDocument Apply(XDocument document, MigrationContext context);
}

