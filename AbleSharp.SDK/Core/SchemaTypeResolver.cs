using System;
using System.Collections.Generic;
using System.Linq;
using AbleSharp.Lib;
using AbleSharp.Lib.Schema;

namespace AbleSharp.SDK;

public static class SchemaTypeResolver
{
    private static readonly Dictionary<string, Type?> SchemaTypes = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object Sync = new();

    static SchemaTypeResolver()
    {
        RebuildSchemaMappings();
    }

    /// <summary>
    /// Rebuilds cached schema mappings based on the current catalog.
    /// </summary>
    public static void Refresh()
    {
        lock (Sync)
        {
            SchemaTypes.Clear();
            RebuildSchemaMappings();
        }
    }

    public static Type GetRootType()
    {
        // Always return our core AbletonProject type
        return typeof(AbletonProject);
    }

    public static string GetSchemaNamespace(string version)
    {
        var sanitizedVersion = "v" + version.Replace('.', '_');
        return $"{SchemaCatalog.SchemaNamespaceRoot}.{sanitizedVersion}";
    }

    private static void RegisterSchemaMapping(string minorVersion, string schemaNamespace)
    {
        var assembly = typeof(AbletonProject).Assembly;
        var probeTypeName = $"{schemaNamespace}.AbletonDevicePreset";
        var schemaType = assembly.GetType(probeTypeName, throwOnError: false, ignoreCase: false);
        SchemaTypes[minorVersion] = schemaType;
    }

    public static bool IsVersionSupported(string minorVersion)
    {
        lock (Sync)
        {
            return SchemaCatalog.SupportedVersions.Any(v => string.Equals(v.VersionId, minorVersion, StringComparison.OrdinalIgnoreCase) && SchemaTypes.TryGetValue(v.VersionId, out var type) && type is not null);
        }
    }

    public static IReadOnlyList<AbletonVersion> GetSupportedVersions()
    {
        lock (Sync)
        {
            return SchemaCatalog.SupportedVersions
                .Where(v => SchemaTypes.ContainsKey(v.VersionId) && SchemaTypes[v.VersionId] is not null)
                .Select(v => new AbletonVersion(v.DisplayName, v.VersionId, v.NamespaceSuffix))
                .ToArray();
        }
    }

    private static void RebuildSchemaMappings()
    {
        SchemaTypes["AbletonProject"] = typeof(AbletonProject);

        foreach (var version in SchemaCatalog.SupportedVersions)
        {
            RegisterSchemaMapping(version.VersionId, version.Namespace);
        }
    }
}