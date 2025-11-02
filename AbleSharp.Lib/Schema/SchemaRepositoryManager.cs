using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AbleSharp.Lib.Schema;

/// <summary>
/// Provides runtime management for <see cref="SchemaRepository"/> including dynamic schema registration and change monitoring.
/// </summary>
public sealed class SchemaRepositoryManager : IDisposable
{
    private readonly object _sync = new();
    private readonly SchemaRepositoryManagerOptions _options;
    private SchemaRepositoryOptions _repositoryOptions;
    private SchemaRepository _current;
    private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new(StringComparer.OrdinalIgnoreCase);
    private Timer? _reloadTimer;
    private string? _pendingReason;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaRepositoryManager"/> class.
    /// </summary>
    public SchemaRepositoryManager(SchemaRepositoryOptions? options = null, SchemaRepositoryManagerOptions? managerOptions = null)
    {
        _repositoryOptions = (options ?? new SchemaRepositoryOptions()).Clone();
        _options = managerOptions ?? new SchemaRepositoryManagerOptions();
        _current = new SchemaRepository(_repositoryOptions);

        if (_options.WatchForChanges)
        {
            InitializeWatchers();
        }
    }

    /// <summary>
    /// Gets the current repository instance.
    /// </summary>
    public SchemaRepository Current => Volatile.Read(ref _current);

    /// <summary>
    /// Occurs when the repository is reloaded due to configuration or file-system changes.
    /// </summary>
    public event EventHandler<SchemaRepositoryChangedEventArgs>? RepositoryChanged;

    /// <summary>
    /// Registers an additional schema root directory for descriptor resolution.
    /// </summary>
    public void RegisterSchemaRoot(string schemaRoot)
    {
        if (string.IsNullOrWhiteSpace(schemaRoot))
        {
            throw new ArgumentNullException(nameof(schemaRoot));
        }

        var normalized = NormalizeDirectory(schemaRoot);
        if (!Directory.Exists(normalized))
        {
            throw new DirectoryNotFoundException($"Schema root '{schemaRoot}' does not exist.");
        }

        lock (_sync)
        {
            if (_repositoryOptions.SchemaRoots.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            _repositoryOptions.SchemaRoots.Add(normalized);
            Reload(SchemaRepositoryChangeKind.SchemaRootAdded, normalized);
            WatchDirectory(normalized);
        }
    }

    /// <summary>
    /// Registers an additional manifest file that should be merged into the repository.
    /// </summary>
    public void RegisterManifest(string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
        {
            throw new ArgumentNullException(nameof(manifestPath));
        }

        var normalized = NormalizeFile(manifestPath);
        if (!File.Exists(normalized))
        {
            throw new FileNotFoundException($"Manifest '{manifestPath}' does not exist.", manifestPath);
        }

        lock (_sync)
        {
            if (_repositoryOptions.AdditionalManifestPaths.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            _repositoryOptions.AdditionalManifestPaths.Add(normalized);
            Reload(SchemaRepositoryChangeKind.ManifestAdded, normalized);
            WatchDirectory(Path.GetDirectoryName(normalized));
        }
    }

    /// <summary>
    /// Forces the repository to reload from its current options.
    /// </summary>
    public void Refresh()
    {
        lock (_sync)
        {
            Reload(SchemaRepositoryChangeKind.Reloaded, null);
        }
    }

    private void Reload(SchemaRepositoryChangeKind kind, string? path)
    {
        var previous = _current;
        var replacement = new SchemaRepository(_repositoryOptions);
        Volatile.Write(ref _current, replacement);
        OnRepositoryChanged(kind, path, previous, replacement);
    }

    private void InitializeWatchers()
    {
        foreach (var root in _repositoryOptions.SchemaRoots)
        {
            WatchDirectory(root);
        }

        foreach (var manifest in _repositoryOptions.AdditionalManifestPaths)
        {
            WatchDirectory(Path.GetDirectoryName(manifest));
        }

        foreach (var manifest in Current.ManifestPaths)
        {
            WatchDirectory(Path.GetDirectoryName(manifest));
        }
    }

    private void WatchDirectory(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        var normalized = NormalizeDirectory(directory);
        if (!Directory.Exists(normalized))
        {
            return;
        }

        if (_watchers.ContainsKey(normalized))
        {
            return;
        }

        if (!_options.WatchForChanges)
        {
            return;
        }

        var watcher = new FileSystemWatcher(normalized)
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };

        FileSystemEventHandler handler = (sender, args) => ScheduleReload(args.FullPath);
        RenamedEventHandler renameHandler = (sender, args) => ScheduleReload(args.FullPath);

        watcher.Changed += handler;
        watcher.Created += handler;
        watcher.Deleted += handler;
        watcher.Renamed += renameHandler;
        watcher.Filter = "*.*";
        watcher.EnableRaisingEvents = true;

        if (_watchers.TryAdd(normalized, watcher))
        {
            _options.Log?.Invoke($"Watching schema directory '{normalized}'.");
        }
        else
        {
            watcher.Dispose();
        }
    }

    private void ScheduleReload(string changedPath)
    {
        lock (_sync)
        {
            _pendingReason = changedPath;
            _reloadTimer?.Dispose();
            _reloadTimer = new Timer(_ => OnWatchedChange(), null, _options.ChangeDebounceInterval, Timeout.InfiniteTimeSpan);
        }
    }

    private void OnWatchedChange()
    {
        string? reason;

        lock (_sync)
        {
            reason = _pendingReason;
            _pendingReason = null;
        }

        if (reason is null)
        {
            return;
        }

        lock (_sync)
        {
            Reload(SchemaRepositoryChangeKind.FileChanged, reason);
        }
    }

    private void OnRepositoryChanged(SchemaRepositoryChangeKind kind, string? path, SchemaRepository previous, SchemaRepository current)
    {
        var args = new SchemaRepositoryChangedEventArgs(kind, path, current, previous);
        RepositoryChanged?.Invoke(this, args);
    }

    private static string NormalizeDirectory(string path) => Path.GetFullPath(path);

    private static string NormalizeFile(string path) => Path.GetFullPath(path);

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.Dispose();
        }

        _watchers.Clear();
        _reloadTimer?.Dispose();
        _current = null!;
    }
}

/// <summary>
/// Provides configuration for <see cref="SchemaRepositoryManager"/>.
/// </summary>
public sealed class SchemaRepositoryManagerOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the manager should watch for schema file changes.
    /// </summary>
    public bool WatchForChanges { get; set; }

    /// <summary>
    /// Gets or sets the debounce interval applied before reloading after a file change.
    /// </summary>
    public TimeSpan ChangeDebounceInterval { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets an optional logging callback invoked for diagnostic messages.
    /// </summary>
    public Action<string>? Log { get; set; }
}

/// <summary>
/// Describes the reason for a schema repository reload.
/// </summary>
public enum SchemaRepositoryChangeKind
{
    /// <summary>
    /// The repository was reloaded explicitly.
    /// </summary>
    Reloaded,

    /// <summary>
    /// A new schema root directory was registered.
    /// </summary>
    SchemaRootAdded,

    /// <summary>
    /// A new manifest file was registered.
    /// </summary>
    ManifestAdded,

    /// <summary>
    /// A watched file changed on disk.
    /// </summary>
    FileChanged
}

/// <summary>
/// Provides data for <see cref="SchemaRepositoryManager.RepositoryChanged"/>.
/// </summary>
public sealed class SchemaRepositoryChangedEventArgs : EventArgs
{
    internal SchemaRepositoryChangedEventArgs(SchemaRepositoryChangeKind kind, string? affectedPath, SchemaRepository current, SchemaRepository? previous)
    {
        Kind = kind;
        AffectedPath = affectedPath;
        Current = current;
        Previous = previous;
    }

    /// <summary>
    /// Gets the reason for the repository change.
    /// </summary>
    public SchemaRepositoryChangeKind Kind { get; }

    /// <summary>
    /// Gets the path associated with the change, when available.
    /// </summary>
    public string? AffectedPath { get; }

    /// <summary>
    /// Gets the current repository instance.
    /// </summary>
    public SchemaRepository Current { get; }

    /// <summary>
    /// Gets the previous repository instance, when applicable.
    /// </summary>
    public SchemaRepository? Previous { get; }
}

