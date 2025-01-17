namespace AbleSharp.SDK.Options;

/// <summary>
/// Options for creating a new project
/// </summary>
public class ProjectCreationOptions : SdkOptions
{
    /// <summary>
    /// Target Ableton Live version for the new project
    /// </summary>
    public string? TargetVersion { get; set; }

    /// <summary>
    /// Initial project tempo
    /// </summary>
    public decimal Tempo { get; set; } = 120;

    /// <summary>
    /// Whether to create default audio and MIDI tracks
    /// </summary>
    public bool CreateDefaultTracks { get; set; } = true;
}

/// <summary>
/// Options for saving a project
/// </summary>
public class ProjectSaveOptions : SdkOptions
{
    /// <summary>
    /// Whether to compress the project file
    /// </summary>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// Whether to create a backup of any existing file
    /// </summary>
    public bool CreateBackup { get; set; } = true;
}

/// <summary>
/// Options for loading a project
/// </summary>
public class ProjectOpenOptions : SdkOptions
{
}

/// <summary>
/// Options for exporting a project
/// </summary>
public class ProjectExportOptions : ProjectSaveOptions
{
    /// <summary>
    /// How to handle features not supported in target version
    /// </summary>
    public FeatureHandling UnsupportedFeatures { get; set; } = FeatureHandling.RemoveWithWarnings;
}

/// <summary>
/// Options for merging projects
/// </summary>
public class ProjectMergeOptions : SdkOptions
{
    /// <summary>
    /// How to handle naming conflicts
    /// </summary>
    public ConflictResolution NamingConflicts { get; set; } = ConflictResolution.Rename;

    /// <summary>
    /// Whether to preserve track colors from source projects
    /// </summary>
    public bool PreserveColors { get; set; } = true;

    /// <summary>
    /// Whether to merge scenes from all projects
    /// </summary>
    public bool MergeScenes { get; set; } = true;
}