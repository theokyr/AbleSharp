using System.Collections.Concurrent;
using System.Threading;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Provides contextual information for migration execution.
/// </summary>
public sealed class MigrationContext
{
    public MigrationContext(SchemaRepository repository, CancellationToken cancellationToken = default)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        CancellationToken = cancellationToken;
        Items = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the schema repository used for descriptor resolution.
    /// </summary>
    public SchemaRepository Repository { get; }

    /// <summary>
    /// Gets the cancellation token used to abort long-running migrations.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets a value indicating whether migrations should run in dry-run mode.
    /// </summary>
    public bool IsDryRun { get; set; }

    /// <summary>
    /// Gets an items dictionary for sharing state between migration steps.
    /// </summary>
    public ConcurrentDictionary<string, object?> Items { get; }
}

