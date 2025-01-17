using System;
using System.Collections.Generic;
using AbleSharp.Lib;

namespace AbleSharp.SDK;

public static class SchemaTypeResolver
{
    private static readonly Dictionary<string, Type?> SchemaTypes = new();

    // Single source of truth for supported versions
    private static readonly AbletonVersion[] SupportedVersions =
    [
        new("11.0", "11.0_11202", "v11_0_11202"), 
        new("12.0", "12.0_12049", "v12_0_12049"),
        new("12.1", "12.0_12120", "v12_0_12120")
    ];

    static SchemaTypeResolver()
    {
        SchemaTypes["AbletonProject"] = typeof(AbletonProject);

        // Register base mapping
        RegisterSchemaMapping("11.0_11202", "AbleSharp.Lib.Schema.v11_0_11202");
        RegisterSchemaMapping("12.0_12049", "AbleSharp.Lib.Schema.v12_0_12049");
        RegisterSchemaMapping("12.0_12120", "AbleSharp.Lib.Schema.v12_0_12120");
    }

    public static Type GetRootType()
    {
        // Always return our core AbletonProject type
        return typeof(AbletonProject);
    }

    public static string GetSchemaNamespace(string version)
    {
        var sanitizedVersion = "v" + version.Replace('.', '_');
        return $"AbleSharp.Lib.Schema.{sanitizedVersion}";
    }

    private static void RegisterSchemaMapping(string minorVersion, string schemaNamespace)
    {
        try
        {
            // Attempt to load the schema assembly type
            var assembly = typeof(AbletonProject).Assembly;
            var schemaType = assembly.GetType(schemaNamespace);
            SchemaTypes[minorVersion] = schemaType;
        }
        catch 
        {
            SchemaTypes[minorVersion] = null;
        }
    }

    public static bool IsVersionSupported(string minorVersion)
    {
        return SupportedVersions.Any(v => v.MinorVersion == minorVersion);
    }

    public static IReadOnlyList<AbletonVersion> GetSupportedVersions()
    {
        // Only return versions that actually have schema types
        return SupportedVersions.Where(v => IsVersionSupported(v.MinorVersion)).ToArray();
    }
}