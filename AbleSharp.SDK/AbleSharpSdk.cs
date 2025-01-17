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

    private AbleSharpSdk() { }

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
            if (!string.IsNullOrEmpty(options.TargetVersion))
                project.MinorVersion = options.TargetVersion;

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
    /// <param name="targetVersion">Version to export to (e.g. "12.0_12049")</param>
    /// <param name="options">Optional export settings</param>
    public void ExportProject(AbletonProject project, string path, string targetVersion, ProjectExportOptions? options = null)
    {
        options ??= new ProjectExportOptions();

        if (project == null)
            throw new ArgumentNullException(nameof(project));

        if (string.IsNullOrEmpty(targetVersion))
            throw new ArgumentNullException(nameof(targetVersion));

        try
        {
            // Verify version is supported
            if (!SchemaTypeResolver.IsVersionSupported(targetVersion))
                throw new NotSupportedException($"Target version {targetVersion} is not supported");

            // TODO: Handle UnsupportedFeatures based on options.UnsupportedFeatures
            // TODO: Implement CollectSamples if options.CollectSamples is true

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
            // TODO: Pass merge options to AbletonProjectMerger
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
    public string GetProjectDump(AbletonProject project) =>
        AbletonProjectDumper.DebugDumpProject(project);

    /// <summary>
    /// Gets whether a specific Ableton Live version is supported
    /// </summary>
    /// <param name="version">The version string (e.g. "12.0_12049")</param>
    public bool IsVersionSupported(string version) => 
        SchemaTypeResolver.IsVersionSupported(version);
}