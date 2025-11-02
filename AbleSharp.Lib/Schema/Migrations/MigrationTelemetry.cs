using System.Diagnostics;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Provides telemetry callbacks for migration planning and execution.
/// </summary>
public interface IMigrationTelemetry
{
    /// <summary>
    /// Invoked after a migration plan has been computed.
    /// </summary>
    void OnPlanCreated(SchemaMigrationPlan plan);

    /// <summary>
    /// Invoked immediately before a migration step is executed.
    /// </summary>
    void OnStepExecuting(SchemaMigrationPlan plan, ISchemaMigration migration, int index, int total);

    /// <summary>
    /// Invoked after a migration step completes successfully.
    /// </summary>
    void OnStepSucceeded(SchemaMigrationPlan plan, ISchemaMigration migration, TimeSpan duration, int index, int total);

    /// <summary>
    /// Invoked after the entire migration plan completes successfully.
    /// </summary>
    void OnPlanSucceeded(SchemaMigrationPlan plan, TimeSpan totalDuration);

    /// <summary>
    /// Invoked when a migration step fails.
    /// </summary>
    void OnPlanFailed(SchemaMigrationPlan plan, ISchemaMigration? migration, Exception exception);
}

/// <summary>
/// Provides helper implementations and factories for migration telemetry.
/// </summary>
public static class MigrationTelemetry
{
    /// <summary>
    /// Gets a telemetry implementation that performs no operations.
    /// </summary>
    public static IMigrationTelemetry Disabled { get; } = new NullMigrationTelemetry();

    private sealed class NullMigrationTelemetry : IMigrationTelemetry
    {
        public void OnPlanCreated(SchemaMigrationPlan plan) { }

        public void OnPlanFailed(SchemaMigrationPlan plan, ISchemaMigration? migration, Exception exception) { }

        public void OnPlanSucceeded(SchemaMigrationPlan plan, TimeSpan totalDuration) { }

        public void OnStepExecuting(SchemaMigrationPlan plan, ISchemaMigration migration, int index, int total) { }

        public void OnStepSucceeded(SchemaMigrationPlan plan, ISchemaMigration migration, TimeSpan duration, int index, int total) { }
    }
}

