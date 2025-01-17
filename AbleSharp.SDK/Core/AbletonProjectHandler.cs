using AbleSharp.SDK.Factories;

namespace AbleSharp.SDK;

using Lib;

public static class AbletonProjectHandler
{
    public static AbletonProject LoadFromFile(string filePath)
    {
        // Detect schema version
        var schemaVersion = SchemaVersionDetector.DetectFromFile(filePath);

        if (!SchemaTypeResolver.IsVersionSupported(schemaVersion)) throw new Exception($"Unsupported Ableton version: {schemaVersion}");

        // Load directly into AbletonProject
        return AbletonSchemaLoader.LoadFromFile(filePath, schemaVersion);
    }

    public static void SaveToFile(AbletonProject project, string filePath)
    {
        // If no target version specified, use same as source
        if (!SchemaTypeResolver.IsVersionSupported(project.MinorVersion)) throw new Exception($"Unsupported Ableton version: {project.MinorVersion}");

        // Save directly from AbletonProject
        AbletonSchemaWriter.SaveToFile(project, filePath);
    }

    public static AbletonProject CreateBlankProject()
    {
        return AbletonProjectFactory.CreateBlankProject();
    }
}