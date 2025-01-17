using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AbleSharp.Lib;

public class SchemaGenerator
{
    private class PropertyInfo
    {
        public string OriginalName { get; set; }
        public string XmlElementName { get; set; }
        public string PropertyType { get; set; }
    }

    private class ClassInfo
    {
        public string ClassName { get; set; }
        public HashSet<string> PropertyNames { get; set; } = new();
        public HashSet<string> BaseTypes { get; set; } = new();
    }

    private readonly Dictionary<string, ClassInfo> classProperties = new();
    private readonly HashSet<string> generatedClasses = new HashSet<string>();
    private readonly HashSet<string> reservedNames = new HashSet<string>();
    private readonly HashSet<string> processedTypes = new HashSet<string>();
    private readonly StringBuilder output = new StringBuilder();
    private int indentLevel = 0;

    public SchemaGenerator()
    {
        InitializeReservedNames();
    }

    private void InitializeReservedNames()
    {
        var keywords = new[]
        {
            "string", "int", "float", "double", "object", "class", "event",
            "base", "params", "namespace", "interface", "delegate", "public",
            "private", "protected", "internal", "readonly", "virtual", "override",
            "abstract", "static", "new", "using", "checked", "unchecked", "const",
            "typeof", "sizeof", "void", "volatile", "enum", "struct"
        };

        foreach (var keyword in keywords)
        {
            reservedNames.Add(keyword);
        }
    }

    public void GenerateForMinorVersion(string minorVersionString, string schemaXml, string outputPath)
    {
        var sanitizedVersion = "v" + minorVersionString.Replace('.', '_');

        // 2. Build a namespace: e.g. "Ableton.Lib.Schema.v12_0_12049"
        var targetNamespace = $"Ableton.Lib.Schema.{sanitizedVersion}";
        
        var doc = XDocument.Parse(schemaXml);
        var schema = doc.Descendants("AbletonSchema").First();

        WriteFileHeader(targetNamespace);

        // Generate base types first
        GenerateBaseTypes();

        // First pass: collect all type information
        var typeDefinitions = schema.Elements()
            .Where(e => !IsIgnoredType(e.Name.LocalName))
            .ToList();

        // Second pass: generate classes
        foreach (var element in typeDefinitions)
        {
            var typeName = element.Name.LocalName;
            if (!generatedClasses.Contains(typeName) && !processedTypes.Contains(typeName))
            {
                GenerateClass(element, typeDefinitions);
                processedTypes.Add(typeName);
            }
        }

        WriteLine("}"); // Close namespace
        File.WriteAllText(outputPath, output.ToString());
    }

    private void WriteFileHeader(string targetNamespace)
    {
        WriteLine("// This file was automatically generated from Ableton Live schema");
        WriteLine("// Do not modify this file manually");
        WriteLine("");
        WriteLine("using System;");
        WriteLine("using System.Collections.Generic;");
        WriteLine("using System.Xml.Serialization;");
        WriteLine("using AbleSharp.Lib;");
        WriteLine("using AbleSharp.Lib.Schema.Common;");
        WriteLine("");
        WriteLine($"namespace {targetNamespace}");
        WriteLine("{");
        indentLevel++;
    }

    private string MapType(string classType, string typeValue, List<XElement> allTypes)
    {
        // Handle array types first
        if (classType.Contains("Array"))
        {
            return MapArrayType(classType);
        }

        // Handle generic types
        if (classType.Contains("<"))
        {
            var match = Regex.Match(classType, @"(\w+)<(.+)>");
            if (match.Success)
            {
                var genericType = match.Groups[1].Value;
                var innerType = match.Groups[2].Value;
                return $"{genericType}<{MapType(innerType, typeValue, allTypes)}>";
            }
        }

        // Handle RemoteableArray specially
        if (classType == "RemoteableArray")
        {
            var arrayType = typeValue.Split(' ').Last();
            return $"RemoteableArray<{arrayType}>";
        }

        // Map basic types
        switch (classType)
        {
            case "Int":
            case "RemoteableInt":
                return "int";
            case "Int64":
            case "RemoteableInt64":
                return "long";
            case "Float":
            case "RemoteableFloat":
            case "TimeableFloat":
            case "UserFloat":
                return "float";
            case "Double":
            case "RemoteableDouble":
                return "double";
            case "String":
            case "RemoteableString":
                return "string";
            case "Bool":
            case "RemoteableBool":
            case "TimeableBool":
                return "bool";
            case "PythonListWrapper":
                return "object";
        }

        // For complex types, ensure they are generated
        if (!generatedClasses.Contains(classType))
        {
            var complexType = allTypes.FirstOrDefault(t => t.Name.LocalName == classType);
            if (complexType != null && !processedTypes.Contains(classType))
            {
                GenerateClass(complexType, allTypes);
                processedTypes.Add(classType);
            }
        }

        return classType;
    }

    private string MapArrayType(string arrayType)
    {
        return arrayType switch
        {
            "Array8" or "ArrayU8" => "byte[]",
            "ArrayFloat" => "float[]",
            _ => "object[]"
        };
    }

    private void GenerateProperty(XElement propertyElement, List<XElement> allTypes, string className)
    {
        var originalName = propertyElement.Name.LocalName;
        var classAttr = propertyElement.Attribute("Class");
        var typeAttr = propertyElement.Attribute("Type");
        var internalNameAttr = propertyElement.Attribute("InternalName");

        if (classAttr == null || typeAttr == null)
            return;

        var propertyName = GetUniquePropertyName(originalName, className);
        var propertyType = MapType(classAttr.Value, typeAttr.Value, allTypes);

        // Generate documentation
        WriteLine("/// <summary>");
        WriteLine($"/// Gets or sets the {SplitCamelCase(originalName)}");
        WriteLine("/// </summary>");

        // Add XML attributes
        var xmlElementName = internalNameAttr?.Value ?? originalName;
        WriteLine($"[XmlElement(\"{xmlElementName}\")]");
        WriteLine($"public {propertyType} {propertyName} {{ get; set; }}");
        WriteLine("");
    }

    private void GenerateClass(XElement classElement, List<XElement> allTypes)
    {
        var className = classElement.Name.LocalName;
        if (generatedClasses.Contains(className))
            return;

        generatedClasses.Add(className);

        // Class documentation
        WriteLine("/// <summary>");
        WriteLine($"/// Represents an Ableton {className} element");
        WriteLine("/// </summary>");
        WriteLine($"[XmlRoot(\"{className}\")]");
        WriteLine($"public class {className}");
        WriteLine("{");
        indentLevel++;

        // Generate properties
        foreach (var propertyElement in classElement.Elements())
        {
            GenerateProperty(propertyElement, allTypes, className);
        }

        indentLevel--;
        WriteLine("}");
        WriteLine("");
    }

    private string GetUniquePropertyName(string baseName, string className)
    {
        var classInfo = classProperties.GetOrAdd(className, _ => new ClassInfo
        {
            ClassName = className
        });

        string propertyName = NormalizePropertyName(baseName, className);
        string uniqueName = propertyName;
        int counter = 1;

        while (classInfo.PropertyNames.Contains(uniqueName))
        {
            uniqueName = $"{propertyName}{counter++}";
        }

        classInfo.PropertyNames.Add(uniqueName);
        return uniqueName;
    }

    private string NormalizePropertyName(string name, string className)
    {
        if (name.Contains("."))
        {
            name = Regex.Replace(name, @"\.\d+$", match => match.Value.Replace(".", ""));
            name = string.Join("", name.Split('.')
                .Select(part => char.ToUpperInvariant(part[0]) + part.Substring(1)));
        }

        name += "Property";

        return name;
    }

    private string SplitCamelCase(string str)
    {
        return Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
    }

    private bool IsIgnoredType(string typeName)
    {
        return typeName == "PythonListWrapper";
    }

    private void WriteLine(string line)
    {
        output.AppendLine(new string(' ', indentLevel * 4) + line);
    }

    private void GenerateBaseTypes()
    {
        // Base types implementation remains the same
        WriteLine("/// <summary>");
        WriteLine("/// Represents a slot that can hold any type T");
        WriteLine("/// </summary>");
        WriteLine("public class Slot<T>");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public T Value { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        // Generate Array types
        WriteLine("/// <summary>");
        WriteLine("/// Represents an array of 8-bit unsigned integers");
        WriteLine("/// </summary>");
        WriteLine("public class Array8");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public byte[] Data { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        WriteLine("/// <summary>");
        WriteLine("/// Represents an array of bytes");
        WriteLine("/// </summary>");
        WriteLine("public class ArrayU8");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public byte[] Data { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        WriteLine("/// <summary>");
        WriteLine("/// Represents an array of floats");
        WriteLine("/// </summary>");
        WriteLine("public class ArrayFloat");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public float[] Data { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        WriteLine("/// <summary>");
        WriteLine("/// Represents a remoting array of type T");
        WriteLine("/// </summary>");
        WriteLine("public class RemoteableArray<T>");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public T[] Items { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        WriteLine("/// <summary>");
        WriteLine("/// Represents a rectangle with x, y coordinates");
        WriteLine("/// </summary>");
        WriteLine("public class Rect");
        WriteLine("{");
        indentLevel++;
        WriteLine("[XmlElement]");
        WriteLine("public int X { get; set; }");
        WriteLine("[XmlElement]");
        WriteLine("public int Y { get; set; }");
        WriteLine("[XmlElement]");
        WriteLine("public int Width { get; set; }");
        WriteLine("[XmlElement]");
        WriteLine("public int Height { get; set; }");
        indentLevel--;
        WriteLine("}");
        WriteLine("");

        // Add standard Ableton types
        WriteLine("public class RemoteableSlot { }");
        WriteLine("public class RemoteableList { }");
        WriteLine("public class RemoteableKeyMidi { }");
        WriteLine("public class RemoteableEnum { }");
        WriteLine("public class RemoteableDocumentColor { }");
        WriteLine("public class TimeableEnum { }");
        WriteLine("public class RangedRemoteableInt { }");
        WriteLine("public class Routable { }");
        WriteLine("public class TimeInSamplesOrMs { }");
        WriteLine("public class DeviceChainContainerName { }");
        WriteLine("public class UserFloatModulationTarget { }");
        WriteLine("public class ClassId { public int Value { get; set; } }");
        WriteLine("public class EditableDeviceChain { }");
        WriteLine("public class PresetRef { }");
        WriteLine("public class PluginInfo { }");
        WriteLine("public class SequencerDevice { }");
        WriteLine("public class Compound { }");
        WriteLine("public class Preset { }");
        WriteLine("public class GroupDeviceBranchPreset { }");
        WriteLine("public class PtrBase { }");
        WriteLine("public class ListAutomation { }");
        WriteLine("public class SampleSlot { }");
        WriteLine("public class CollisionModTarget { }");
        WriteLine("public class SimplerPlayer { }");
        WriteLine("public class SimplerPitch { }");
        WriteLine("public class SimplerFilterHolder { }");
        WriteLine("public class SimplerShaperHolder { }");
        WriteLine("public class SimplerVolumeAndPan { }");
        WriteLine("public class SimplerAuxEnvelopeHolder { }");
        WriteLine("public class SimplerLfoHolder { }");
        WriteLine("public class SimplerAuxLfoHolder { }");
        WriteLine("public class SimplerModDst { }");
        WriteLine("public class SimplerModDstWithFeedback { }");
        WriteLine("public class SimplerGlobals { }");
        WriteLine("public class SimplerViewSettings { }");
        WriteLine("public class SimplerSlicing { }");
        WriteLine("public class SideChain { }");
        WriteLine("public class SideChainEq { }");
        WriteLine("public class DrumZoneSettings { }");
        WriteLine("public class SingleTimeSignatureManager { }");
        WriteLine("public class VolumeEnvelope { }");
        WriteLine("public class MacroVariations { }");
        WriteLine("public class RemoteableViewData { }");
        WriteLine("public class TimeableModulationTarget { }");
        WriteLine("public class MidiSideChain { }");
        WriteLine("public class OperatorTimeableModConnection { }");
        WriteLine("public class UltraAnalogEnvelope { }");
        WriteLine("public class ClipEnvelopes { }");
        WriteLine("public class ClipGrooveSettings { }");
        WriteLine("public class SidechainRoutingDeviceHelper { }");
        WriteLine("public class Vst3Uid { }");
        WriteLine("public class PluginParameterSettings { }");
        WriteLine("");

        // Mark all base types as generated
        generatedClasses.Add("Slot");
        generatedClasses.Add("Array8");
        generatedClasses.Add("ArrayU8");
        generatedClasses.Add("ArrayFloat");
        generatedClasses.Add("RemoteableArray");
        generatedClasses.Add("Rect");
        generatedClasses.Add("RemoteableSlot");
        generatedClasses.Add("RemoteableList");
        generatedClasses.Add("RemoteableKeyMidi");
        generatedClasses.Add("RemoteableEnum");
        generatedClasses.Add("RemoteableDocumentColor");
        generatedClasses.Add("TimeableEnum");
        generatedClasses.Add("RangedRemoteableInt");
        generatedClasses.Add("Routable");
        generatedClasses.Add("TimeInSamplesOrMs");
        generatedClasses.Add("DeviceChainContainerName");
        generatedClasses.Add("UserFloatModulationTarget");
        generatedClasses.Add("ClassId");
        generatedClasses.Add("EditableDeviceChain");
        generatedClasses.Add("PresetRef");
        generatedClasses.Add("PluginInfo");
        generatedClasses.Add("SequencerDevice");
        generatedClasses.Add("Compound");
        generatedClasses.Add("Preset");
        generatedClasses.Add("GroupDeviceBranchPreset");
        generatedClasses.Add("PtrBase");
        generatedClasses.Add("ListAutomation");
        generatedClasses.Add("SampleSlot");
        generatedClasses.Add("CollisionModTarget");
        generatedClasses.Add("SimplerPlayer");
        generatedClasses.Add("SimplerPitch");
        generatedClasses.Add("SimplerFilterHolder");
        generatedClasses.Add("SimplerShaperHolder");
        generatedClasses.Add("SimplerVolumeAndPan");
        generatedClasses.Add("SimplerAuxEnvelopeHolder");
        generatedClasses.Add("SimplerLfoHolder");
        generatedClasses.Add("SimplerAuxLfoHolder");
        generatedClasses.Add("SimplerModDst");
        generatedClasses.Add("SimplerModDstWithFeedback");
        generatedClasses.Add("SimplerGlobals");
        generatedClasses.Add("SimplerViewSettings");
        generatedClasses.Add("SimplerSlicing");
        generatedClasses.Add("SideChain");
        generatedClasses.Add("SideChainEq");
        generatedClasses.Add("DrumZoneSettings");
        generatedClasses.Add("SingleTimeSignatureManager");
        generatedClasses.Add("VolumeEnvelope");
        generatedClasses.Add("MacroVariations");
        generatedClasses.Add("RemoteableViewData");
        generatedClasses.Add("TimeableModulationTarget");
        generatedClasses.Add("MidiSideChain");
        generatedClasses.Add("OperatorTimeableModConnection");
        generatedClasses.Add("UltraAnalogEnvelope");
        generatedClasses.Add("ClipEnvelopes");
        generatedClasses.Add("ClipGrooveSettings");
        generatedClasses.Add("SidechainRoutingDeviceHelper");
        generatedClasses.Add("Vst3Uid");
        generatedClasses.Add("PluginParameterSettings");
    }
}

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dict,
        TKey key,
        Func<TKey, TValue> valueFactory)
    {
        if (!dict.TryGetValue(key, out TValue value))
        {
            value = valueFactory(key);
            dict.Add(key, value);
        }

        return value;
    }
}