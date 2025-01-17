using AbleSharp.SDK.Factories;

namespace AbleSharp.SDK;

using Lib;

public static class AbletonProjectHandler
{
    public static AbletonProject LoadFromFile(string filePath)
    {
        // Detect schema version
        var schemaVersion = SchemaVersionDetector.DetectFromFile(filePath);
        
        if (!SchemaTypeResolver.IsVersionSupported(schemaVersion))
        {
            throw new Exception($"Unsupported Ableton version: {schemaVersion}");
        }

        // Load directly into AbletonProject
        return AbletonSchemaLoader.LoadFromFile(filePath, schemaVersion);
    }

    public static void SaveToFile(AbletonProject project, string filePath, string targetSchemaVersion = null)
    {
        // If no target version specified, use same as source
        targetSchemaVersion ??= project.MinorVersion;

        if (!SchemaTypeResolver.IsVersionSupported(targetSchemaVersion))
        {
            throw new Exception($"Unsupported Ableton version: {targetSchemaVersion}");
        }

        // Save directly from AbletonProject
        AbletonSchemaWriter.SaveToFile(project, filePath);
    }

    public static AbletonProject CreateBlankProject()
    {
        return AbletonProjectFactory.CreateBlankProject();
    }
}