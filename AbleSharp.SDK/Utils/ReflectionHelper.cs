using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ableton.SDK.Utils;

public static class ReflectionHelper
{
    /// <summary>
    /// Gets the value of a property (by name) via reflection.
    /// Returns null if property not found or inaccessible.
    /// </summary>
    public static object? GetPropValue(object obj, string propertyName)
    {
        if (obj == null) return null;
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        return prop?.CanRead == true ? prop.GetValue(obj) : null;
    }

    /// <summary>
    /// Sets the value of a property (by name) via reflection.
    /// Ignores if property not found, or if types are incompatible.
    /// </summary>
    public static void SetPropValue(object obj, string propertyName, object? value)
    {
        if (obj == null) return;
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop?.CanWrite == true)
            // Optional: check type compatibility
            if (value == null || prop.PropertyType.IsInstanceOfType(value))
                prop.SetValue(obj, value);
        // else you might try to convert the type or skip
    }

    /// <summary>
    /// Creates a new instance of a given type using parameterless constructor.
    /// </summary>
    public static object? CreateInstance(Type t)
    {
        try
        {
            return Activator.CreateInstance(t);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to append all items from sourceList into targetList (if both are IList).
    /// Both must be non-null, and of the same or compatible item type.
    /// </summary>
    public static void ConcatLists(object sourceList, object targetList)
    {
        if (sourceList == null || targetList == null) return;
        if (sourceList is IList sList && targetList is IList tList)
            foreach (var item in sList)
                tList.Add(item);
    }
}