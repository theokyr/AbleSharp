using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text.Json;

namespace AbleSharp.Lib.Schema;

/// <summary>
/// Provides descriptor discovery, caching, and integrity validation for Ableton schema resources.
/// </summary>
public sealed class SchemaRepository
{
    private const string ManifestEnvironmentVariable = "ABLESHARP_SCHEMA_MANIFEST";
    private const string AdditionalManifestEnvironmentVariable = "ABLESHARP_SCHEMA_MANIFESTS";
    private const string SchemaRootsEnvironmentVariable = "ABLESHARP_SCHEMA_ROOTS";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly ImmutableList<SchemaDescriptor> _descriptors;
    private readonly ImmutableDictionary<string, SchemaDescriptor> _descriptorLookup;
    private readonly ImmutableList<string> _schemaRoots;
    private readonly ImmutableList<string> _manifestPaths;

    /// <summary>
    /// Gets the resolved primary manifest path used by the repository.
    /// </summary>
    public string ManifestPath { get; }

    /// <summary>
    /// Gets the manifest paths that were merged when constructing the repository.
    /// </summary>
    public IReadOnlyList<string> ManifestPaths => _manifestPaths;

    /// <summary>
    /// Gets the collection of schema root directories used for descriptor resolution.
    /// </summary>
    public IReadOnlyList<string> SchemaRoots => _schemaRoots;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaRepository"/> class.
    /// </summary>
    /// <param name="options">Optional configuration.</param>
    /// <exception cref="FileNotFoundException">Thrown when the manifest cannot be resolved.</exception>
    /// <exception cref="InvalidOperationException">Thrown when manifest contents are invalid.</exception>
    public SchemaRepository(SchemaRepositoryOptions? options = null)
    {
        options ??= new SchemaRepositoryOptions();

        ManifestPath = ResolveManifestPath(options.ManifestPath);
        _manifestPaths = ResolveManifestPaths(options, ManifestPath).ToImmutableList();
        _schemaRoots = ResolveSchemaRoots(options, _manifestPaths).ToImmutableList();

        (_descriptors, _descriptorLookup) = LoadDescriptors(_manifestPaths);

        if (options.ValidateOnLoad)
        {
            ValidateAllDescriptors();
        }
    }

    /// <summary>
    /// Gets all registered schema descriptors in manifest order.
    /// </summary>
    public IReadOnlyList<SchemaDescriptor> GetDescriptors() => _descriptors;

    /// <summary>
    /// Attempts to retrieve a descriptor by version identifier.
    /// </summary>
    /// <param name="versionId">The Ableton schema version identifier.</param>
    /// <param name="descriptor">The resolved descriptor.</param>
    /// <returns><c>true</c> when the descriptor was found; otherwise <c>false</c>.</returns>
    public bool TryGetDescriptor(string versionId, out SchemaDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(versionId))
        {
            descriptor = null!;
            return false;
        }

        return _descriptorLookup.TryGetValue(versionId, out descriptor!);
    }

    /// <summary>
    /// Retrieves a descriptor by version identifier or throws when not found.
    /// </summary>
    /// <param name="versionId">The Ableton schema version identifier.</param>
    /// <returns>The resolved descriptor.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the descriptor is missing.</exception>
    public SchemaDescriptor GetDescriptor(string versionId)
    {
        if (!TryGetDescriptor(versionId, out var descriptor))
        {
            throw new KeyNotFoundException($"Schema descriptor '{versionId}' was not found in manifest '{ManifestPath}'.");
        }

        return descriptor;
    }

    /// <summary>
    /// Validates a descriptor and returns a detailed result.
    /// </summary>
    /// <param name="descriptor">The descriptor to validate.</param>
    public SchemaValidationResult ValidateDescriptor(SchemaDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        var fullPath = descriptor.FullPath;

        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
        {
            return SchemaValidationResult.Missing(descriptor);
        }

        var fileInfo = new FileInfo(fullPath);
        if (fileInfo.Length != descriptor.SizeBytes)
        {
            return SchemaValidationResult.SizeMismatch(descriptor, fileInfo.Length);
        }

        using var stream = File.OpenRead(fullPath);
        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant();

        if (!string.Equals(hash, descriptor.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            return SchemaValidationResult.HashMismatch(descriptor, hash, fileInfo.Length);
        }

        return SchemaValidationResult.Valid(descriptor);
    }

    /// <summary>
    /// Ensures that the descriptor identified by <paramref name="versionId"/> is present and valid.
    /// </summary>
    /// <param name="versionId">The Ableton schema version identifier.</param>
    /// <returns>The resolved descriptor.</returns>
    /// <exception cref="InvalidOperationException">Thrown when integrity validation fails.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the descriptor is missing.</exception>
    public SchemaDescriptor EnsureDescriptor(string versionId)
    {
        var descriptor = GetDescriptor(versionId);
        var result = ValidateDescriptor(descriptor);

        if (!result.IsValid)
        {
            throw new InvalidOperationException(result.Message);
        }

        return descriptor;
    }

    private void ValidateAllDescriptors()
    {
        var failures = new List<SchemaValidationResult>();

        foreach (var descriptor in _descriptors)
        {
            var result = ValidateDescriptor(descriptor);
            if (!result.IsValid)
            {
                failures.Add(result);
            }
        }

        if (failures.Count == 0)
        {
            return;
        }

        var message = string.Join(Environment.NewLine, failures.Select(f => f.Message));
        throw new InvalidOperationException($"One or more schema descriptors failed integrity validation:{Environment.NewLine}{message}");
    }

    private (ImmutableList<SchemaDescriptor> list, ImmutableDictionary<string, SchemaDescriptor> lookup) LoadDescriptors(IReadOnlyList<string> manifestPaths)
    {
        var aggregated = new List<SchemaDescriptor>();
        var lookup = new Dictionary<string, SchemaDescriptor>(StringComparer.OrdinalIgnoreCase);

        foreach (var manifestPath in manifestPaths)
        {
            using var stream = File.OpenRead(manifestPath);
            var descriptors = JsonSerializer.Deserialize<List<SchemaDescriptor>>(stream, SerializerOptions)
                             ?? throw new InvalidOperationException($"Schema manifest '{manifestPath}' did not contain a valid descriptor array.");

            foreach (var descriptor in descriptors)
            {
                descriptor.FullPath = ResolveFilePath(descriptor.FileName);

                if (!lookup.TryAdd(descriptor.VersionId, descriptor))
                {
                    throw new InvalidOperationException($"Duplicate schema descriptor '{descriptor.VersionId}' detected while loading manifest '{manifestPath}'.");
                }

                aggregated.Add(descriptor);
            }
        }

        return (aggregated.ToImmutableList(), lookup.ToImmutableDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase));
    }

    private string? ResolveFilePath(string fileName)
    {
        foreach (var root in _schemaRoots)
        {
            var candidate = Path.Combine(root, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> ResolveSchemaRoots(SchemaRepositoryOptions options, IReadOnlyList<string> manifestPaths)
    {
        var roots = new List<string>();

        // Environment variable overrides (semicolon-separated to support Windows and Unix path separators).
        var envRoots = Environment.GetEnvironmentVariable(SchemaRootsEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(envRoots))
        {
            foreach (var root in envRoots.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                roots.Add(root);
            }
        }

        // Explicit options.
        foreach (var root in options.SchemaRoots)
        {
            roots.Add(root);
        }

        // Directories containing manifests.
        foreach (var manifestPath in manifestPaths)
        {
            var manifestDirectory = Path.GetDirectoryName(manifestPath);
            if (!string.IsNullOrWhiteSpace(manifestDirectory))
            {
                roots.Add(manifestDirectory);
            }
        }

        // Default fallback: probe parent directories of the application base.
        foreach (var candidate in EnumerateDefaultSchemaRoots())
        {
            roots.Add(candidate);
        }

        return roots
            .Select(NormalizePath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(Directory.Exists)
            .ToList();
    }

    private static IEnumerable<string> EnumerateDefaultSchemaRoots()
    {
        var baseDir = AppContext.BaseDirectory;
        var current = baseDir;

        for (var i = 0; i < 6; i++)
        {
            if (string.IsNullOrWhiteSpace(current))
            {
                yield break;
            }

            yield return Path.Combine(current, "resources", "schemas");
            current = Directory.GetParent(current)?.FullName ?? string.Empty;
        }
    }

    private static string ResolveManifestPath(string? manifestPath)
    {
        var explicitPath = Environment.GetEnvironmentVariable(ManifestEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            manifestPath = explicitPath;
        }

        if (!string.IsNullOrWhiteSpace(manifestPath))
        {
            var candidate = NormalizePath(manifestPath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            throw new FileNotFoundException($"Schema manifest '{manifestPath}' could not be found.", candidate);
        }

        foreach (var candidate in EnumerateCandidateManifests())
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException("Unable to locate schema manifest. Set ABLESHARP_SCHEMA_MANIFEST or provide SchemaRepositoryOptions.ManifestPath.");
    }

    private static IEnumerable<string> ResolveManifestPaths(SchemaRepositoryOptions options, string primaryManifest)
    {
        var paths = new List<string>();

        paths.Add(primaryManifest);

        // Environment-specified additional manifests.
        var envAdditional = Environment.GetEnvironmentVariable(AdditionalManifestEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(envAdditional))
        {
            foreach (var candidate in envAdditional.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var normalized = NormalizePath(candidate);
                if (File.Exists(normalized))
                {
                    paths.Add(normalized);
                }
            }
        }

        foreach (var candidate in options.AdditionalManifestPaths)
        {
            var normalized = NormalizePath(candidate);
            if (File.Exists(normalized))
            {
                paths.Add(normalized);
            }
        }

        return paths
            .Select(NormalizePath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> EnumerateCandidateManifests()
    {
        var baseDir = AppContext.BaseDirectory;
        var current = baseDir;

        for (var i = 0; i < 6; i++)
        {
            if (string.IsNullOrWhiteSpace(current))
            {
                yield break;
            }

            yield return Path.Combine(current, "resources", "schemas", "schema-descriptors.json");
            current = Directory.GetParent(current)?.FullName ?? string.Empty;
        }
    }

    private static string NormalizePath(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
    }
}

