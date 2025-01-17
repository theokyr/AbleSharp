using AbleSharp.Lib;
using AbleSharp.SDK.Factories;
using AbleSharp.SDK.Options;

namespace AbleSharp.SDK;

/// <summary>
/// Main entry point for working with Ableton Live project files
/// </summary>
public class AbleSharpSdk
{
    private static readonly Lazy<AbleSharpSdk> _instance = new(() => new AbleSharpSdk());

    /// <summary>
    /// Gets the singleton instance of AbleSharpSdk
    /// </summary>
    public static AbleSharpSdk Instance => _instance.Value;

    /// <summary>
    /// List of all supported Ableton Live versions
    /// </summary>
    public readonly List<AbletonVersion> AvailableVersions;

    /// <summary>
    /// Gets the most recent supported version
    /// </summary>
    public AbletonVersion LatestVersion => AvailableVersions[^1];

    private AbleSharpSdk()
    {
        SchemaTypeResolver.GetSupportedVersions()
            .Select(v => new AbletonVersion(v.DisplayName, v.MinorVersion, v.NamespaceVersion))
            .ToList();
    }

    /// <summary>
    /// Creates a new empty Ableton Live project
    /// </summary>
    /// <param name="options">Optional settings for project creation</param>
    /// <returns>The new project</returns>
    public AbletonProject CreateProject(ProjectCreationOptions? options = null)
    {
        options ??= new ProjectCreationOptions();

        try
        {
            var project = AbletonProjectFactory.CreateBlankProject();

            // Apply options
            if (!string.IsNullOrEmpty(options.TargetMinorVersion))
                project.MinorVersion = options.TargetMinorVersion;

            if (project.LiveSet?.MainTrack?.DeviceChain?.Mixer?.Tempo != null)
                project.LiveSet.MainTrack.DeviceChain.Mixer.Tempo.Manual.Val = options.Tempo;

            // Only create default tracks if requested
            if (!options.CreateDefaultTracks)
                project.LiveSet?.Tracks?.Clear();

            return project;
        }
        catch (Exception ex) when (options.ErrorHandling == ErrorHandling.ReturnEmpty)
        {
            options.Logger?.Invoke($"Error creating project: {ex.Message}");
            return AbletonProjectFactory.CreateBlankProject();
        }
    }

    /// <summary>
    /// Opens an existing Ableton Live project file
    /// </summary>
    /// <param name="path">Path to the .als file</param>
    /// <param name="options">Optional settings for loading the project</param>
    /// <returns>The loaded project</returns>
    public AbletonProject OpenProject(string path, ProjectOpenOptions? options = null)
    {
        options ??= new ProjectOpenOptions();

        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("Ableton project file not found", path);

        try
        {
            return AbletonProjectHandler.LoadFromFile(path);
        }
        catch (Exception ex) when (options.ErrorHandling == ErrorHandling.ReturnEmpty)
        {
            options.Logger?.Invoke($"Error loading project: {ex.Message}");
            return AbletonProjectFactory.CreateBlankProject();
        }
    }

    /// <summary>
    /// Saves a project to disk
    /// </summary>
    /// <param name="project">The project to save</param>
    /// <param name="path">Where to save the project</param>
    /// <param name="options">Optional settings for saving</param>
    public void SaveProject(AbletonProject project, string path, ProjectSaveOptions? options = null)
    {
        options ??= new ProjectSaveOptions();

        if (project == null)
            throw new ArgumentNullException(nameof(project));

        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        try
        {
            // Create backup if requested
            if (options.CreateBackup && File.Exists(path))
            {
                var backupPath = $"{path}.bak";
                File.Copy(path, backupPath, true);
                options.Logger?.Invoke($"Created backup at {backupPath}");
            }

            AbletonProjectHandler.SaveToFile(project, path);
            options.Logger?.Invoke($"Project saved successfully to {path}");
        }
        catch (Exception ex)
        {
            options.Logger?.Invoke($"Error saving project: {ex.Message}");
            throw; // Always throw save errors
        }
    }

    /// <summary>
    /// Exports project to a specific Ableton version
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="path">Where to save the exported project</param>
    /// <param name="targetVersion">AbletonVersion to export to</param>
    /// <param name="options">Optional export settings</param>
    public void ExportProject(AbletonProject project, string path, AbletonVersion targetVersion, ProjectExportOptions? options = null)
    {
        options ??= new ProjectExportOptions();

        if (project == null)
            throw new ArgumentNullException(nameof(project));

        if (targetVersion == null)
            throw new ArgumentNullException(nameof(targetVersion));

        try
        {
            // Verify AbletonVersion is supported
            if (!SchemaTypeResolver.IsVersionSupported(targetVersion.MinorVersion))
                throw new NotSupportedException($"Target AbletonVersion {targetVersion.DisplayName} is not supported");

            // Set target version
            project.MinorVersion = targetVersion.NamespaceVersion;

            // Save using standard save options
            SaveProject(project, path, options);
        }
        catch (Exception ex) when (options.ErrorHandling == ErrorHandling.ReturnEmpty)
        {
            options.Logger?.Invoke($"Error exporting project: {ex.Message}");
            // Don't save anything if export fails and we're in ReturnEmpty mode
        }
    }

    /// <summary>
    /// Merges multiple Ableton Live projects into a single project
    /// </summary>
    /// <param name="projects">The projects to merge</param>
    /// <param name="options">Optional settings for handling the merge</param>
    /// <returns>The merged project</returns>
    public AbletonProject MergeProjects(IEnumerable<AbletonProject> projects, ProjectMergeOptions? options = null)
    {
        options ??= new ProjectMergeOptions();

        if (projects == null)
            throw new ArgumentNullException(nameof(projects));

        var projectList = projects.ToList();
        if (!projectList.Any())
            throw new ArgumentException("At least one project must be provided for merging", nameof(projects));

        try
        {
            return AbletonProjectMerger.MergeProjects(projectList);
        }
        catch (Exception ex) when (options.ErrorHandling == ErrorHandling.ReturnEmpty)
        {
            options.Logger?.Invoke($"Error merging projects: {ex.Message}");
            return AbletonProjectFactory.CreateBlankProject();
        }
    }

    /// <summary>
    /// Gets a debug dump of the project structure
    /// </summary>
    public string GetProjectDump(AbletonProject project)
    {
        return AbletonProjectDumper.DebugDumpProject(project);
    }

    /// <summary>
    /// Gets whether a specific Ableton Live AbletonVersion is supported
    /// </summary>
    /// <param name="version">The AbletonVersion to check support for</param>
    public bool IsVersionSupported(AbletonVersion version)
    {
        return SchemaTypeResolver.IsVersionSupported(version.NamespaceVersion);
    }
}

/// <summary>
/// Available Ableton versions that can be used for projects
/// </summary>
public class AbletonVersion(string displayName, string minorVersion, string namespaceVersion)
{
    public string DisplayName { get; } = displayName;
    public string MinorVersion { get; } = minorVersion;
    public string NamespaceVersion { get; } = namespaceVersion;

    public override string ToString() => DisplayName;
}