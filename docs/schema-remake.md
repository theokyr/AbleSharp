# Schema Migration Strategy for AbleSharp

## Executive Summary
- **Problem**: Schema-specific code generation is time-consuming, brittle, and blocks support for new Ableton Live versions.
- **Goal**: Enable AbleSharp to read and write any `.als` file by standardizing on a canonical model backed by a migration pipeline that can target arbitrary schema versions.
- **Scope**: Applies to `AbleSharp.Lib`, `AbleSharp.SDK`, `AbleSharp.CLI`, and `AbleSharp.GUI`, with initial schema assets stored in `resources/schemas` and optional user-provided schema roots.
- **Outcome**: Consistent developer experience, predictable migrations, and a foundation for auto-discovered Ableton schemas.

## Guiding Principles
- **Canonical Model First**: Domain classes represent the latest supported Ableton schema; migrations bridge older/newer versions.
- **Explicit Migrations**: Every schema transition is encoded as forward and (where possible) backward migrations with validation checks.
- **Composable Tooling**: Schema repository, migration engine, and code generation are independently testable services.
- **Automation Over Manual Effort**: Regeneration, validation, and drift detection are automated in CI.
- **Extensibility**: Users can register additional schema sources without modifying AbleSharp binaries.

## Current State Assessment
- Schemas are manually integrated per version with partially generated types and hand-written adjustments.
- No global registry for schema versions or lineage; migrations are ad-hoc.
- Canonical domain classes diverge from the latest Ableton schema, causing serialization gaps.
- CLI/SDK lack commands for schema inspection or migration operations.
- Tests cover a limited set of versions and do not exercise multi-hop migrations.

## Target Architecture
### Components
- `SchemaRepository`
  - Discovers schema files from `resources/schemas` and configurable search paths.
  - Extracts metadata (version number, variant, checksum) and exposes normalized descriptors.
- `SchemaGraph`
  - Maintains directed edges between versions; supports queries for migration paths and reachability.
- `CanonicalModel`
  - Represents latest Ableton schema; used by Lib/SDK.
  - Backed by generated serialization helpers to map to/from concrete schema representations.
- `MigrationEngine`
  - Executes ordered migrations, optionally using an intermediate representation (IR).
  - Provides diagnostics, dry-run, and validation facilities.
- `SchemaTooling`
  - CLI commands for listing schemas, diffing versions, executing migrations.
  - CI jobs for schema integrity and codegen validation.

### Data Flow
1. Load `.als` file -> detect schema version via header.
2. Resolve schema metadata through `SchemaRepository`.
3. If version differs from canonical, select migration path via `SchemaGraph`.
4. `MigrationEngine` converts file data into canonical model (or desired target version).
5. Domain operations occur on canonical model; export reverses the flow to requested version.

## Implementation Phases

### Phase 0 – Preparation 
- Catalog all existing schemas, generation scripts, and known divergences.
- Define naming/versioning conventions for schema files and migrations.
- Establish acceptance criteria for “canonical model matches schema”.

### Phase 1 – Schema Repository & Version Detection
- **Status: Complete** – Schema discovery, manifest ingestion, and CLI tooling ship in 0.0.6.
  - ✅ Implemented `SchemaRepository` with caching/integrity validation backed by `resources/schemas/schema-descriptors.json`.
  - ✅ `SchemaVersionDetector` inspects Ableton headers and resolves registered descriptors (`SchemaRepositoryTests`).
  - ✅ `ablesharp schemas list` exposes filters/validation flags for operators (`AbleSharp.CLI/Program.cs`).
  - ✅ Test coverage: repository load/validation + detector scenarios (`SchemaRepositoryTests`).

### Phase 2 – Canonical Model Alignment
- **Status: Complete** – Canonical schema `12.0_12203` drives code generation, SDK defaults, and guardrail tests.
  - ✅ `SchemaCatalog` centralizes canonical identifiers, change count, and namespace suffix.
  - ✅ Generated binding `Schema/Generated/v12_0_12203.cs` is part of the Lib assembly and factory defaults now target it (`AbletonProjectFactory`, `SchemaTypeResolver`).
  - ✅ Canonical alignment validation via `CanonicalAlignmentTests` (descriptor availability + generated types) and CLI migration smoke tests.
  - ✅ Documentation updated with canonical mapping notes and migration guidance (this document).

#### Canonical Mapping Notes
- Canonical version (`12.0_12203`) is the SDK default for project creation/export.
- `SchemaCatalog` exposes canonical metadata for tooling and migrations.
- Generated namespace `Ableton.Lib.Schema.v12_0_12203` is validated by tests to prevent drift.

### Phase 3 – Migration Framework 
- **Status: Complete** – Default forward/backward migrations, planning engine, and CLI tooling are shipping.
  - ✅ `ISchemaMigration`, metadata records, execution context, and `MigrationEngine` ship with graph-based planning and structured exceptions.
  - ✅ `DefaultSchemaMigrations` provides bidirectional metadata upgrades (11.0 ↔ 12.0 ↔ 12.1 ↔ canonical `12.0_12203`).
  - ✅ CLI `ablesharp migrate` command supports auto-detect, `--from/--to`, dry-run and plan visualization.
  - ✅ Tests: `MigrationEngineTests` cover forward/backward planning plus execution; canonical alignment tests exercise generated bindings.
  - ✅ Documentation updated (this plan) to describe migration behavior and usage.

### Phase 4 – Code Generation Modernization
- **Status: Complete** – Roslyn incremental generator (`AbleSharp.Schema.Generator`) emits schema bindings and catalog metadata during build.
  - ✅ Shared generation library encapsulates schema plan parsing and code emission.
  - ✅ Generated namespaces now live under `AbleSharp.Lib.Schema.Generated.*`, removing stale tracked code.
  - ✅ `AbleSharp.Lib` consumes the generator via `AdditionalFiles`, keeping manifests as single source of truth.
  - ✅ Tests ensure canonical namespace loads and generation plan round-trips.
- ⏭️ CI work items: add pipeline job to diff-check generated sources (tracked separately).

### Phase 5 – Extensibility & User Schemas
- **Status: Complete** – Runtime schema management shipped across Lib/SDK/CLI.
  - ✅ `SchemaRepositoryManager` supports registering roots/manifests, change watching, and events.
  - ✅ CLI gains `--schema-root`, `--manifest`, and opt-in telemetry for migrations.
  - ✅ SDK exposes schema registration/refresh APIs and refreshes supported version cache.
  - ✅ Migration engine publishes telemetry hooks; console implementation logs optional insights.
- 📄 Documentation updated (this doc) with runtime schema flow; user import guide pending follow-up.

### Phase 6 – Documentation, Tooling, and Rollout
- Write migration authoring guide, schema contribution guidelines, and troubleshooting docs.
- Provide sample migrations and fixtures in `extras/` covering complex cases.
- Coordinate release notes and SDK change log updates.
- Plan beta rollout with early adopters; gather feedback, iterate.

## Deliverables by Component
- `AbleSharp.Lib`: Canonical domain alignment, migration engine, generated helpers, expanded tests.
- `AbleSharp.SDK`: APIs for selecting target version, registering schema roots, exposing migration diagnostics.
- `AbleSharp.CLI`: Schema management commands, migration tooling, improved error messages.
- `AbleSharp.GUI`: Version-aware loading/saving, user notifications for migrations, settings for preferred target version.
- `resources/schemas`: Structured metadata, organized directory per version, integrity hashes.
- `docs/`: Updated developer docs (`schema-remake.md`, migration authoring guide, CLI reference updates).

## Testing Strategy
- **Unit Tests**: Repository operations, migration transformations, schema metadata parsing.
- **Integration Tests**: End-to-end migrations across multiple versions using real `.als` fixtures; GUI load/save smoke tests.
- **Regression Tests**: Snapshot or property-based tests ensuring migrations do not drop critical data.
- **Performance Benchmarks**: Measure migration time/memory on large projects; set guardrails and optimize hotspots.
- **CI Enhancements**: Schema checksum validation, code generation drift detection, migration coverage reports.

## Risk Analysis & Mitigations
- **Migration Gaps**: If forward/backward migration is impossible, mark schema as partially supported and block release until resolved; maintain dashboard of coverage.
- **Schema Drift**: Nightly job regenerates canonical comparison report; fail CI when drift detected.
- **User-Supplied Schemas**: Validate schema format before ingestion; sandbox parsing to prevent crashes; expose feature flag.
- **Performance Regression**: Introduce performance gates, incremental migrations, and caching for repeated conversions.
- **Complexity Creep**: Maintain clear ownership per component, enforce documentation and code reviews for migrations.

## Success Criteria
- AbleSharp loads and saves `.als` files across all supported Ableton versions without manual schema edits.
- Adding a new schema requires no changes outside schema assets and migrations.
- CI detects schema drift and migration regressions within minutes.
- Developers have clear playbooks for authoring migrations and diagnosing issues.
- Users can plug in their own Ableton installation schemas with minimal configuration.


