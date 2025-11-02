using System.Collections.Generic;
using System.Xml.Linq;
using AbleSharp.Lib.Schema.Migrations;
using NUnit.Framework;

namespace AbleSharp.Lib.Schema.Tests;

[TestFixture]
public class MigrationEngineTests
{
    [Test]
    public void PlanAndMigrateForward_ToCanonicalVersion_UpdatesHeader()
    {
        var repository = new SchemaRepository();
        var engine = new MigrationEngine(repository, DefaultSchemaMigrations.All);

        var plan = engine.Plan("11.0_11202", SchemaCatalog.CanonicalVersionId, SchemaMigrationDirection.Forward);

        Assert.That(plan.IsIdentity, Is.False);
        Assert.That(plan.Steps, Has.Count.EqualTo(3));

        var document = XDocument.Parse("<Ableton MajorVersion=\"5\" MinorVersion=\"11.0_11202\" SchemaChangeCount=\"11202\" />");

        var migrated = engine.Migrate(document, "11.0_11202", SchemaCatalog.CanonicalVersionId);

        Assert.Multiple(() =>
        {
            Assert.That(migrated.Root, Is.Not.Null);
            Assert.That(migrated.Root!.Attribute("MinorVersion")?.Value, Is.EqualTo(SchemaCatalog.CanonicalVersionId));
            Assert.That(migrated.Root.Attribute("SchemaChangeCount")?.Value, Is.EqualTo(SchemaCatalog.CanonicalSchemaChangeCount.ToString()));
        });
    }

    [Test]
    public void PlanBackward_FromCanonical_ReturnsExpectedSteps()
    {
        var repository = new SchemaRepository();
        var engine = new MigrationEngine(repository, DefaultSchemaMigrations.All);

        var plan = engine.Plan(SchemaCatalog.CanonicalVersionId, "11.0_11202", SchemaMigrationDirection.Backward);

        Assert.That(plan.IsIdentity, Is.False);
        Assert.That(plan.Steps, Has.Count.EqualTo(3));
    }

    [Test]
    public void Plan_InvalidSource_Throws()
    {
        var repository = new SchemaRepository();
        var engine = new MigrationEngine(repository, DefaultSchemaMigrations.All);

        Assert.Throws<SchemaMigrationException>(() => engine.Plan("99.0_1", SchemaCatalog.CanonicalVersionId, SchemaMigrationDirection.Forward));
    }

    [Test]
    public void Execute_EmitsTelemetryCallbacks()
    {
        var repository = new SchemaRepository();
        var telemetry = new RecordingTelemetry();
        var engine = new MigrationEngine(repository, DefaultSchemaMigrations.All, telemetry);

        var plan = engine.Plan("11.0_11202", SchemaCatalog.CanonicalVersionId, SchemaMigrationDirection.Forward);
        Assert.That(telemetry.PlanCreatedCount, Is.EqualTo(1));

        var document = XDocument.Parse("<Ableton MajorVersion=\"5\" MinorVersion=\"11.0_11202\" SchemaChangeCount=\"11202\" />");

        engine.Migrate(document, "11.0_11202", SchemaCatalog.CanonicalVersionId);

        Assert.Multiple(() =>
        {
            Assert.That(telemetry.PlanSucceededCount, Is.EqualTo(1));
            Assert.That(telemetry.StepExecuting.Count, Is.EqualTo(plan.Steps.Count));
            Assert.That(telemetry.StepSucceeded.Count, Is.EqualTo(plan.Steps.Count));
            Assert.That(telemetry.Failures, Is.Empty);
        });
    }

    private sealed class RecordingTelemetry : IMigrationTelemetry
    {
        public int PlanCreatedCount { get; private set; }
        public int PlanSucceededCount { get; private set; }
        public List<string> StepExecuting { get; } = new();
        public List<string> StepSucceeded { get; } = new();
        public List<string> Failures { get; } = new();

        public void OnPlanCreated(SchemaMigrationPlan plan)
        {
            PlanCreatedCount++;
        }

        public void OnPlanFailed(SchemaMigrationPlan plan, ISchemaMigration? migration, Exception exception)
        {
            Failures.Add(exception.Message);
        }

        public void OnPlanSucceeded(SchemaMigrationPlan plan, TimeSpan totalDuration)
        {
            PlanSucceededCount++;
        }

        public void OnStepExecuting(SchemaMigrationPlan plan, ISchemaMigration migration, int index, int total)
        {
            StepExecuting.Add(migration.Metadata.Description);
        }

        public void OnStepSucceeded(SchemaMigrationPlan plan, ISchemaMigration migration, TimeSpan duration, int index, int total)
        {
            StepSucceeded.Add(migration.Metadata.Description);
        }
    }
}

