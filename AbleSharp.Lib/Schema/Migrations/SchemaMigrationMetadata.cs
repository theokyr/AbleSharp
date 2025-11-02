namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Describes a concrete migration step between two Ableton schema versions.
/// </summary>
/// <param name="SourceVersionId">Source Ableton schema version identifier.</param>
/// <param name="TargetVersionId">Target Ableton schema version identifier.</param>
/// <param name="Direction">Migration direction (forward/backward).</param>
/// <param name="Description">Human-readable description of the migration.</param>
public sealed record SchemaMigrationMetadata(
    string SourceVersionId,
    string TargetVersionId,
    SchemaMigrationDirection Direction,
    string Description);

