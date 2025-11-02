using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Executes version migrations using a set of registered migration steps.
/// </summary>
public sealed class MigrationEngine
{
    private readonly SchemaRepository _repository;
    private readonly ImmutableArray<ISchemaMigration> _migrations;
    private readonly SchemaGraph _graph;
    private readonly IMigrationTelemetry _telemetry;

    public MigrationEngine(SchemaRepository repository, IEnumerable<ISchemaMigration> migrations, IMigrationTelemetry? telemetry = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        if (migrations is null)
        {
            throw new ArgumentNullException(nameof(migrations));
        }

        _migrations = migrations.ToImmutableArray();
        if (_migrations.IsDefaultOrEmpty)
        {
            throw new ArgumentException("At least one migration must be provided.", nameof(migrations));
        }

        _graph = new SchemaGraph(_migrations);
        _telemetry = telemetry ?? MigrationTelemetry.Disabled;
    }

    /// <summary>
    /// Plans the migration path between two schema versions.
    /// </summary>
    public SchemaMigrationPlan Plan(string sourceVersionId, string targetVersionId, SchemaMigrationDirection direction)
    {
        var path = _graph.FindPath(sourceVersionId, targetVersionId, direction);

        if (path.Count == 0 && !string.Equals(sourceVersionId, targetVersionId, StringComparison.OrdinalIgnoreCase))
        {
            throw new SchemaMigrationException($"Unable to locate a {direction.ToString().ToLowerInvariant()} migration path from '{sourceVersionId}' to '{targetVersionId}'.");
        }

        var plan = new SchemaMigrationPlan(sourceVersionId, targetVersionId, direction, path);
        _telemetry.OnPlanCreated(plan);
        return plan;
    }

    /// <summary>
    /// Executes a migration plan against the provided document.
    /// </summary>
    public XDocument Execute(SchemaMigrationPlan plan, XDocument document, MigrationContext? context = null)
    {
        if (plan is null)
        {
            throw new ArgumentNullException(nameof(plan));
        }

        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var working = new XDocument(document);
        var effectiveContext = context ?? new MigrationContext(_repository);

        if (plan.IsIdentity)
        {
            _telemetry.OnPlanSucceeded(plan, TimeSpan.Zero);
            return working;
        }

        var planStopwatch = Stopwatch.StartNew();

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            var migration = plan.Steps[index];
            effectiveContext.CancellationToken.ThrowIfCancellationRequested();
            _telemetry.OnStepExecuting(plan, migration, index, plan.Steps.Count);

            var stepStopwatch = Stopwatch.StartNew();

            try
            {
                working = migration.Apply(working, effectiveContext);
                stepStopwatch.Stop();
                _telemetry.OnStepSucceeded(plan, migration, stepStopwatch.Elapsed, index, plan.Steps.Count);
            }
            catch (Exception ex)
            {
                stepStopwatch.Stop();
                _telemetry.OnPlanFailed(plan, migration, ex);
                throw new SchemaMigrationException(
                    $"Migration '{migration.Metadata.Description}' ({migration.Metadata.SourceVersionId} -> {migration.Metadata.TargetVersionId}) failed.",
                    plan,
                    migration,
                    ex);
            }
        }

        planStopwatch.Stop();
        _telemetry.OnPlanSucceeded(plan, planStopwatch.Elapsed);

        return working;
    }

    /// <summary>
    /// Plans and executes a migration between two schema versions.
    /// </summary>
    public XDocument Migrate(XDocument document, string sourceVersionId, string targetVersionId, SchemaMigrationDirection direction = SchemaMigrationDirection.Forward, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var plan = Plan(sourceVersionId, targetVersionId, direction);

        if (plan.IsIdentity)
        {
            return new XDocument(document);
        }

        var context = new MigrationContext(_repository, cancellationToken)
        {
            IsDryRun = dryRun
        };

        return Execute(plan, document, context);
    }
}

