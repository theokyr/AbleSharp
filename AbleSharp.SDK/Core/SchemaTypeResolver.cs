using System;
using System.Collections.Generic;
using AbleSharp.Lib;

namespace AbleSharp.SDK;

public static class SchemaTypeResolver
{
    private static readonly Dictionary<string, Type> SchemaTypes = new();

    static SchemaTypeResolver()
    {
        SchemaTypes["AbletonProject"] = typeof(AbletonProject);

        RegisterSchemaMapping("11.0_11202", "AbleSharp.Lib.Schema.v11_0_11202");
        RegisterSchemaMapping("12.0_12120", "AbleSharp.Lib.Schema.v12_0_12120");
        RegisterSchemaMapping("12.0_12049", "AbleSharp.Lib.Schema.v12_0_12049");
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

    private static void RegisterSchemaMapping(string version, string namespacePrefix)
    {
        SchemaTypes[version] = null; // Just store version mapping
    }

    public static bool IsVersionSupported(string version)
    {
        return SchemaTypes.ContainsKey(version);
    }
}