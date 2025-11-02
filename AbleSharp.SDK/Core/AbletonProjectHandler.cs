using AbleSharp.SDK.Factories;

namespace AbleSharp.SDK;

using Lib;

public static class AbletonProjectHandler
{
    public static AbletonProject LoadFromFile(string filePath, Action<string> logger = null)
    {
        logger?.Invoke($"Loading project from file: {filePath}");
        
        // Detect schema version
        logger?.Invoke("Detecting schema version...");
        var schemaVersion = SchemaVersionDetector.DetectFromFile(filePath);
        logger?.Invoke($"Detected schema version: {schemaVersion}");

        if (!SchemaTypeResolver.IsVersionSupported(schemaVersion))
        {
            var error = $"Unsupported Ableton version: {schemaVersion}";
            logger?.Invoke($"ERROR: {error}");
            throw new Exception(error);
        }
        logger?.Invoke($"Schema version {schemaVersion} is supported");

        try
        {
            // Load directly into AbletonProject
            logger?.Invoke("Loading project into AbletonProject model...");
            var project = AbletonSchemaLoader.LoadFromFile(filePath, schemaVersion);
            logger?.Invoke($"Successfully loaded project: {project.Creator} [{project.MinorVersion}]");
            
            // Log some basic project stats
            logger?.Invoke($"Project details:");
            logger?.Invoke($"- Tracks: {project.LiveSet?.Tracks?.Count ?? 0}");
            logger?.Invoke($"- Scenes: {project.LiveSet?.Scenes?.Count ?? 0}");
            logger?.Invoke($"- Schema changes: {project.SchemaChangeCount}");
            
            return project;
        }
        catch (Exception ex)
        {
            logger?.Invoke($"CRITICAL ERROR loading project: {ex.Message}");
            logger?.Invoke($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public static void SaveToFile(AbletonProject project, string filePath, Action<string> logger = null)
    {
        logger?.Invoke($"Preparing to save project to: {filePath}");

        // If no target version specified, use same as source
        if (!SchemaTypeResolver.IsVersionSupported(project.MinorVersion))
        {
            var error = $"Unsupported Ableton version: {project.MinorVersion}";
            logger?.Invoke($"ERROR: {error}");
            throw new Exception(error);
        }
        logger?.Invoke($"Validated schema version: {project.MinorVersion}");

        try
        {
            // Save directly from AbletonProject
            logger?.Invoke("Starting save operation...");
            AbletonSchemaWriter.SaveToFile(project, filePath);
            logger?.Invoke($"Successfully saved project to: {filePath}");

            // Log save confirmation details
            logger?.Invoke("Save operation completed with:");
            logger?.Invoke($"- Creator: {project.Creator}");
            logger?.Invoke($"- Version: {project.MinorVersion}");
            logger?.Invoke($"- Track count: {project.LiveSet?.Tracks?.Count ?? 0}");
        }
        catch (Exception ex)
        {
            logger?.Invoke($"CRITICAL ERROR saving project: {ex.Message}");
            logger?.Invoke($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public static AbletonProject CreateBlankProject(Action<string> logger = null)
    {
        logger?.Invoke("Creating new blank project...");
        var project = AbletonProjectFactory.CreateBlankProject();
        logger?.Invoke($"Created blank project:");
        logger?.Invoke($"- Version: {project.MinorVersion}");
        logger?.Invoke($"- Creator: {project.Creator}");
        logger?.Invoke($"- Default tracks: {project.LiveSet?.Tracks?.Count ?? 0}");
        return project;
    }
}