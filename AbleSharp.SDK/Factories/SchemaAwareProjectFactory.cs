using System;
using Ableton.SDK.Utils;

namespace Ableton.SDK.Factories
{
    public static class SchemaAwareProjectFactory
    {
        /// <summary>
        /// Creates a "blank" project for the given MinorVersion string, e.g. "12.0_12049".
        /// So it instantiates "Ableton.Lib.Schema.v12_0_12049.AbletonProject" via reflection.
        /// Then sets some minimal default properties.
        /// </summary>
        public static object CreateBlankProject(string minorVersion)
        {
            // 1) Build the namespace
            var sanitizedVersion = "v" + minorVersion.Replace('.', '_');
            var schemaNamespace = $"Ableton.Lib.Schema.{sanitizedVersion}";
            var typeName = $"{schemaNamespace}.AbletonProject";
            
            var rootType = Type.GetType(typeName, throwOnError: false);
            if (rootType == null)
                throw new InvalidOperationException($"Could not find root type {typeName} for MinorVersion={minorVersion}");

            // 2) Create instance
            var projectObj = ReflectionHelper.CreateInstance(rootType);
            if (projectObj == null)
                throw new InvalidOperationException($"Unable to instantiate {typeName}.");

            // 3) Optionally set defaults
            // ReflectionHelper.SetPropValue(projectObj, "MajorVersion", 5);
            ReflectionHelper.SetPropValue(projectObj, "MinorVersion", minorVersion);
            // ReflectionHelper.SetPropValue(projectObj, "Creator", "Some default string");
            // etc.

            // 4) Also create a "LiveSet" property if it doesn't exist:
            var liveSetProp = rootType.GetProperty("LiveSet");
            if (liveSetProp != null && liveSetProp.CanWrite)
            {
                // create instance of that type
                var liveSetObj = ReflectionHelper.CreateInstance(liveSetProp.PropertyType);
                liveSetProp.SetValue(projectObj, liveSetObj);

                // maybe set some default "AutomationMode" property, etc.
                // ReflectionHelper.SetPropValue(liveSetObj, "AutomationMode", false);
            }

            return projectObj;
        }
    }
}
