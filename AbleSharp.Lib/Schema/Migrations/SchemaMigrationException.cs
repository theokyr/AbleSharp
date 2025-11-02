namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Represents errors that occur while planning or executing schema migrations.
/// </summary>
public sealed class SchemaMigrationException : Exception
{
    public SchemaMigrationException(string message)
        : base(message)
    {
    }

    public SchemaMigrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SchemaMigrationException(string message, SchemaMigrationPlan? plan, ISchemaMigration? migration, Exception? innerException = null)
        : base(message, innerException)
    {
        Plan = plan;
        Migration = migration;
    }

    /// <summary>
    /// Gets the migration plan that was being executed when the exception occurred, if available.
    /// </summary>
    public SchemaMigrationPlan? Plan { get; }

    /// <summary>
    /// Gets the migration that failed, if available.
    /// </summary>
    public ISchemaMigration? Migration { get; }
}

