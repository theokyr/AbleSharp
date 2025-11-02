namespace AbleSharp.Lib.Schema;

/// <summary>
/// Provides configuration for <see cref="SchemaRepository"/>.
/// </summary>
public sealed class SchemaRepositoryOptions
{
    /// <summary>
    /// Gets or sets the manifest file path containing schema descriptors.
    /// When null, the repository discovers the manifest relative to <see cref="AppContext.BaseDirectory"/>.
    /// </summary>
    public string? ManifestPath { get; set; }

    /// <summary>
    /// Gets the collection of additional manifest file paths merged with the primary manifest.
    /// </summary>
    public IList<string> AdditionalManifestPaths { get; } = new List<string>();

    /// <summary>
    /// Gets the mutable list of schema root directories used to resolve descriptor file names.
    /// </summary>
    public IList<string> SchemaRoots { get; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the repository validates all descriptors when loading.
    /// </summary>
    public bool ValidateOnLoad { get; set; }

    /// <summary>
    /// Creates a copy of the options instance.
    /// </summary>
    public SchemaRepositoryOptions Clone()
    {
        var clone = new SchemaRepositoryOptions
        {
            ManifestPath = ManifestPath,
            ValidateOnLoad = ValidateOnLoad
        };

        foreach (var root in SchemaRoots)
        {
            clone.SchemaRoots.Add(root);
        }

        foreach (var manifest in AdditionalManifestPaths)
        {
            clone.AdditionalManifestPaths.Add(manifest);
        }

        return clone;
    }
}

