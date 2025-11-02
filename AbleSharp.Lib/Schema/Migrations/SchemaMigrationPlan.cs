using System.Collections.Immutable;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Represents an ordered list of migrations required to move between two schema versions.
/// </summary>
public sealed class SchemaMigrationPlan
{
    public SchemaMigrationPlan(string sourceVersionId, string targetVersionId, SchemaMigrationDirection direction, IEnumerable<ISchemaMigration> steps)
    {
        if (string.IsNullOrWhiteSpace(sourceVersionId))
        {
            throw new ArgumentException("Source version identifier cannot be null or empty.", nameof(sourceVersionId));
        }

        if (string.IsNullOrWhiteSpace(targetVersionId))
        {
            throw new ArgumentException("Target version identifier cannot be null or empty.", nameof(targetVersionId));
        }

        SourceVersionId = sourceVersionId;
        TargetVersionId = targetVersionId;
        Direction = direction;
        Steps = (steps ?? Array.Empty<ISchemaMigration>()).ToImmutableArray();
    }

    /// <summary>
    /// Gets the source schema version identifier.
    /// </summary>
    public string SourceVersionId { get; }

    /// <summary>
    /// Gets the target schema version identifier.
    /// </summary>
    public string TargetVersionId { get; }

    /// <summary>
    /// Gets the direction of the planned migration.
    /// </summary>
    public SchemaMigrationDirection Direction { get; }

    /// <summary>
    /// Gets the ordered migration steps for the plan.
    /// </summary>
    public IReadOnlyList<ISchemaMigration> Steps { get; }

    /// <summary>
    /// Gets a value indicating whether the plan represents an identity transformation.
    /// </summary>
    public bool IsIdentity => Steps.Count == 0;
}

