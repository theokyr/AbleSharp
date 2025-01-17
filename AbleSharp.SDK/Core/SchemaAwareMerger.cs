using System;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using AbleSharp.Lib;

namespace AbleSharp.SDK;

/// <summary>
/// Provides schema-aware merging capabilities between AbleSharp base classes and generated schema types 
/// </summary>
public static class SchemaAwareMerger
{
    // Holds cached property mappings between schema versions and AbleSharp classes 
    private static readonly Dictionary<string, Dictionary<Type, Dictionary<string, PropertyInfo>>> PropertyMappingCache = new();

    /// <summary>
    /// Merges a schema-specific object into an AbleSharp base class
    /// </summary>
    public static T MergeFromSchema<T>(object schemaObject, string schemaVersion) where T : class, new()
    {
        if (schemaObject == null)
            return null;

        // Create instance of target AbleSharp class
        var target = Activator.CreateInstance<T>();
        
        // Get or create property mappings
        var mappings = GetPropertyMappings(typeof(T), schemaObject.GetType(), schemaVersion);

        // Copy matching properties
        foreach (var mapping in mappings)
        {
            try
            {
                var sourceValue = mapping.Value.Item1.GetValue(schemaObject);
                if (sourceValue != null)
                {
                    // Handle collections
                    if (typeof(IList).IsAssignableFrom(mapping.Value.Item2.PropertyType))
                    {
                        var targetList = (IList)Activator.CreateInstance(mapping.Value.Item2.PropertyType);
                        var sourceList = (IList)sourceValue;
                        
                        foreach (var item in sourceList)
                        {
                            var targetItemType = mapping.Value.Item2.PropertyType.GetGenericArguments()[0];
                            var convertedItem = ConvertValue(item, targetItemType, schemaVersion);
                            targetList.Add(convertedItem);
                        }
                        
                        mapping.Value.Item2.SetValue(target, targetList);
                    }
                    else
                    {
                        var convertedValue = ConvertValue(sourceValue, mapping.Value.Item2.PropertyType, schemaVersion); 
                        mapping.Value.Item2.SetValue(target, convertedValue);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other properties
                Console.WriteLine($"Error merging property {mapping.Key}: {ex.Message}");
            }
        }

        return target;
    }

    /// <summary>
    /// Merges an AbleSharp object into a schema-specific type
    /// </summary> 
    public static T MergeToSchema<T>(object ableSharpObject, string schemaVersion) where T : class
    {
        if (ableSharpObject == null)
            return null;

        var target = Activator.CreateInstance<T>();
        var mappings = GetPropertyMappings(ableSharpObject.GetType(), typeof(T), schemaVersion);

        foreach (var mapping in mappings)
        {
            try
            {
                var sourceValue = mapping.Value.Item1.GetValue(ableSharpObject);
                if (sourceValue != null)
                {
                    if (typeof(IList).IsAssignableFrom(mapping.Value.Item2.PropertyType))
                    {
                        var targetList = (IList)Activator.CreateInstance(mapping.Value.Item2.PropertyType);
                        var sourceList = (IList)sourceValue;
                        
                        foreach (var item in sourceList)
                        {
                            var targetItemType = mapping.Value.Item2.PropertyType.GetGenericArguments()[0];
                            var convertedItem = ConvertValue(item, targetItemType, schemaVersion);
                            targetList.Add(convertedItem);
                        }
                        
                        mapping.Value.Item2.SetValue(target, targetList);
                    }
                    else
                    {
                        var convertedValue = ConvertValue(sourceValue, mapping.Value.Item2.PropertyType, schemaVersion);
                        mapping.Value.Item2.SetValue(target, convertedValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error merging property {mapping.Key}: {ex.Message}");
            }
        }

        return target;
    }

    private static Dictionary<string, (PropertyInfo, PropertyInfo)> GetPropertyMappings(Type sourceType, Type targetType, string schemaVersion)
    {
        var cacheKey = $"{sourceType.FullName}_{targetType.FullName}_{schemaVersion}";
        
        if (PropertyMappingCache.TryGetValue(cacheKey, out var cachedMappings))
        {
            var result = new Dictionary<string, (PropertyInfo, PropertyInfo)>();
            foreach (var mapping in cachedMappings[sourceType])
            {
                var targetProp = targetType.GetProperty(mapping.Key);
                if (targetProp != null)
                {
                    result[mapping.Key] = (mapping.Value, targetProp);
                }
            }
            return result;
        }

        var mappings = new Dictionary<string, (PropertyInfo, PropertyInfo)>();
        var sourceProps = sourceType.GetProperties();
        var targetProps = targetType.GetProperties();

        // Map properties based on name and XML attributes
        foreach (var sourceProp in sourceProps)
        {
            var sourceXmlAttrib = sourceProp.GetCustomAttribute<XmlElementAttribute>();
            var sourceName = sourceXmlAttrib?.ElementName ?? sourceProp.Name;

            var targetProp = targetProps.FirstOrDefault(p =>
            {
                var targetXmlAttrib = p.GetCustomAttribute<XmlElementAttribute>();
                var targetName = targetXmlAttrib?.ElementName ?? p.Name;
                return string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase);
            });

            if (targetProp != null)
            {
                mappings[sourceName] = (sourceProp, targetProp);
            }
        }

        // Cache the mappings
        if (!PropertyMappingCache.ContainsKey(cacheKey))
        {
            PropertyMappingCache[cacheKey] = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }
        PropertyMappingCache[cacheKey][sourceType] = mappings.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Item1
        );

        return mappings;
    }

    private static object ConvertValue(object value, Type targetType, string schemaVersion)
    {
        if (value == null)
            return null;

        // Handle enums
        if (targetType.IsEnum && value is string enumStr)
        {
            return Enum.Parse(targetType, enumStr);
        }

        // Handle Value<T> wrapper types 
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Value<>))
        {
            var innerType = targetType.GetGenericArguments()[0];
            var val = ConvertValue(value, innerType, schemaVersion);
            var wrapper = Activator.CreateInstance(targetType);
            targetType.GetProperty("Val")?.SetValue(wrapper, val);
            return wrapper;
        }

        // Handle collections
        if (typeof(IList).IsAssignableFrom(targetType) && value is IList sourceList)
        {
            var targetList = (IList)Activator.CreateInstance(targetType);
            var elementType = targetType.GetGenericArguments()[0];
            foreach (var item in sourceList)
            {
                targetList.Add(ConvertValue(item, elementType, schemaVersion));
            }
            return targetList;
        }

        // Handle complex types
        if (!targetType.IsPrimitive && targetType != typeof(string))
        {
            return MergeFromSchema<object>(value, schemaVersion); 
        }

        // Basic type conversion
        return Convert.ChangeType(value, targetType);
    }
}