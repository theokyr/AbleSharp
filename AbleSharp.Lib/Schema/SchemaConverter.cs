using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace AbleSharp.Schema
{
    public class SchemaConverterService
    {
        private readonly AbletonSchemaConverter _converter;

        public SchemaConverterService()
        {
            _converter = new AbletonSchemaConverter();
        }

        /// <summary>
        /// Reads an Ableton-style XML schema from a file and converts it to XSD.
        /// </summary>
        public string ConvertSchemaFile(string schemaPath)
        {
            if (!File.Exists(schemaPath))
            {
                throw new FileNotFoundException("Schema file not found", schemaPath);
            }

            var schemaXml = File.ReadAllText(schemaPath);

            // Load the Ableton schema
            var doc = new XmlDocument();
            doc.LoadXml(schemaXml);

            // Convert to XSD
            _converter.ConvertToXsd(doc);

            // Return the XSD as a string
            return _converter.SaveToString();
        }

        /// <summary>
        /// Validates a given XML file against a provided XSD schema string.
        /// </summary>
        public bool ValidateAgainstSchema(string xmlPath, string xsdSchema)
        {
            try {
                var schemaSet = new XmlSchemaSet();
                using (var schemaReader = new StringReader(xsdSchema))
                {
                    schemaSet.Add(null, XmlReader.Create(schemaReader));
                }

                var doc = new XmlDocument { Schemas = schemaSet };
                doc.Load(xmlPath);
        
                ValidationEventHandler handler = (s, e) => {
                    if (e.Severity == XmlSeverityType.Error)
                        throw new XmlSchemaValidationException(e.Message);
                };
        
                doc.Validate(handler);
                return true;
            }
            catch (XmlSchemaValidationException)
            {
                return false; 
            }
        }
    }

    public class AbletonSchemaConverter
    {
        private readonly XmlSchemaSet _schemaSet;
        private readonly XmlSchema _schema;

        /// <summary>
        /// Tracks existing named complex types by their "class name" so we don't duplicate.
        /// </summary>
        private readonly Dictionary<string, XmlSchemaComplexType> _complexTypes;

        /// <summary>
        /// Counter for generating unique names for anonymous value types (if needed).
        /// </summary>
        private int _anonymousTypeCounter = 0;

        /// <summary>
        /// Caches basic XSD type mappings for fast lookup.
        /// </summary>
        private static readonly Dictionary<string, XmlQualifiedName> BasicTypeMap = new Dictionary<string, XmlQualifiedName>
        {
            { "Int", new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema") },
            { "Float", new XmlQualifiedName("float",   "http://www.w3.org/2001/XMLSchema") },
            { "Double", new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema") },
            { "Bool", new XmlQualifiedName("boolean",  "http://www.w3.org/2001/XMLSchema") },
            { "String", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema") }
        };

        public AbletonSchemaConverter()
        {
            _schemaSet = new XmlSchemaSet();
            _schema = new XmlSchema
            {
                TargetNamespace = "http://www.ableton.com/schemas/live-set",
                ElementFormDefault = XmlSchemaForm.Qualified
            };
            _complexTypes = new Dictionary<string, XmlSchemaComplexType>();
        }

        /// <summary>
        /// Main entry point for converting an Ableton-style XML schema to an XSD.
        /// </summary>
        public XmlSchema ConvertToXsd(XmlDocument abletonSchema)
        {
            // Find top-level elements inside <AbletonSchema> … </AbletonSchema>
            var classNodes = abletonSchema.SelectNodes("//AbletonSchema/*[local-name() != '#text']");
            foreach (XmlNode classNode in classNodes)
            {
                ProcessClassDefinition(classNode);
            }

            // Create a root element "Ableton" -> references a root type "AbletonType"
            var abletonType = EnsureComplexTypeExists("AbletonType");
            var sequence = new XmlSchemaSequence();

            // Add "LiveSet" as mandatory sub-element
            var liveSetElement = new XmlSchemaElement
            {
                Name = "LiveSet",
                SchemaTypeName = new XmlQualifiedName("LiveSet", _schema.TargetNamespace),
                MinOccurs = 1,
                MaxOccurs = 1
            };
            sequence.Items.Add(liveSetElement);

            // Add required attributes for root <Ableton> element if needed
            var attributes = new[]
            {
                new { Name = "MajorVersion", Type = "integer" },
                new { Name = "MinorVersion", Type = "string" },
                new { Name = "SchemaChangeCount", Type = "integer" },
                new { Name = "Creator", Type = "string" },
                new { Name = "Revision", Type = "string" }
            };

            foreach (var attr in attributes)
            {
                abletonType.Attributes.Add(new XmlSchemaAttribute
                {
                    Name = attr.Name,
                    SchemaTypeName = new XmlQualifiedName(attr.Type, "http://www.w3.org/2001/XMLSchema"),
                    Use = XmlSchemaUse.Required
                });
            }

            abletonType.Particle = sequence;

            // Create the top-level element "Ableton"
            var rootElement = new XmlSchemaElement
            {
                Name = "Ableton",
                SchemaTypeName = new XmlQualifiedName("AbletonType", _schema.TargetNamespace)
            };
            _schema.Items.Add(rootElement);

            _schemaSet.Add(_schema);
            _schemaSet.Compile();

            return _schema;
        }

        /// <summary>
        /// Creates or retrieves an existing named complex type for the given class name.
        /// </summary>
        private XmlSchemaComplexType EnsureComplexTypeExists(string className)
        {
            if (_complexTypes.TryGetValue(className, out var existingType))
            {
                return existingType;
            }

            // Create a brand-new complex type
            var newType = new XmlSchemaComplexType { Name = className };
            _schema.Items.Add(newType);
            _complexTypes[className] = newType;
            return newType;
        }

        /// <summary>
        /// Processes a "class node" from the Ableton schema and generates
        /// a complex type definition for that class.
        /// </summary>
        private void ProcessClassDefinition(XmlNode classNode)
        {
            var className = classNode.Name;

            // Make sure the class has a valid complex type
            var complexType = EnsureComplexTypeExists(className);

            // We'll build or overwrite the <xs:sequence> to hold properties
            var sequence = new XmlSchemaSequence();

            var propertyNodes = classNode.ChildNodes.Cast<XmlNode>()
                .Where(n => n.NodeType == XmlNodeType.Element);

            foreach (var propertyNode in propertyNodes)
            {
                var element = CreateElementFromPropertyNode(propertyNode);
                sequence.Items.Add(element);
            }

            // Attach the sequence to the complex type
            complexType.Particle = sequence;
        }

        /// <summary>
        /// Given a single property node (like <Name Class="String" Type="-4" />),
        /// create a corresponding XSD element.
        /// </summary>
        private XmlSchemaElement CreateElementFromPropertyNode(XmlNode propertyNode)
        {
            var element = new XmlSchemaElement
            {
                Name = propertyNode.Name,
                MinOccurs = 0 // Optional by default
            };

            // Inspect the "Class" and "Type" attributes
            var classAttr = propertyNode.Attributes?["Class"]?.Value;
            var typeAttr = propertyNode.Attributes?["Type"]?.Value;

            if (string.IsNullOrEmpty(classAttr) && string.IsNullOrEmpty(typeAttr))
            {
                // Fallback, treat as "xs:anyType"
                element.SchemaTypeName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
                return element;
            }

            // If we detect something like "Slot<Device>" or "List<Track>"...
            if (!string.IsNullOrEmpty(classAttr) && classAttr.Contains("<"))
            {
                element = HandleGenericClass(element, classAttr);
            }
            else if (!string.IsNullOrEmpty(classAttr))
            {
                // Non-generic class => reference a named complex type
                // Make sure it's defined
                EnsureComplexTypeExists(classAttr);
                element.SchemaTypeName = new XmlQualifiedName(classAttr, _schema.TargetNamespace);
            }

            // If there's a basic type or special type code
            if (typeAttr == "-4")
            {
                // Basic type
                // Example: <Name Class="String" Type="-4" />
                var abletonTypeName = classAttr ?? "String"; // fallback to "String" if somehow missing
                element.SchemaTypeName = MapBasicOrAnyType(abletonTypeName);
            }
            else if (typeAttr == "0")
            {
                // "Complex" = reference a named type, or handle special sub-case like "TimeableBool"
                // If "TimeableBool" or "RemoteableString", etc. => define or reference
                if (!string.IsNullOrEmpty(classAttr))
                {
                    // We already ensured the type above. If it's a known "TimeableX" or "RemoteableX," 
                    // you can do advanced logic or simply do:
                    element.SchemaTypeName = new XmlQualifiedName(classAttr, _schema.TargetNamespace);
                }
                else
                {
                    // fallback
                    element.SchemaTypeName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
                }
            }

            return element;
        }

        /// <summary>
        /// Handles generics like "Slot<Device>" or "List<PluginParameterSettings>" 
        /// to create either single or repeated elements referencing the inner type.
        /// </summary>
        private XmlSchemaElement HandleGenericClass(XmlSchemaElement element, string classAttr)
        {
            // e.g. "Slot<Device>" or "List<PresetRef>"
            var genericMatch = Regex.Match(classAttr, @"(?<container>\w+)<(?<type>\w+)>");
            if (!genericMatch.Success)
            {
                // fallback if it's something unexpected
                element.SchemaTypeName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
                return element;
            }

            var container = genericMatch.Groups["container"].Value;
            var innerType = genericMatch.Groups["type"].Value;

            // Ensure the type is declared
            EnsureComplexTypeExists(innerType);

            switch (container)
            {
                case "List":
                case "RemoteableList":
                case "PythonListWrapper":
                    element.MaxOccursString = "unbounded";
                    element.SchemaTypeName = new XmlQualifiedName(innerType, _schema.TargetNamespace);
                    break;
                case "Slot":
                case "Slot<Device>":
                case "Slot<PresetRef>":
                    // Single item referencing the named type
                    element.SchemaTypeName = new XmlQualifiedName(innerType, _schema.TargetNamespace);
                    break;
                default:
                    element.SchemaTypeName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
                    break;
            }

            return element;
        }

        /// <summary>
        /// Maps "Int", "String", etc. to XSD basic types. Fallback is "xs:anyType".
        /// </summary>
        private XmlQualifiedName MapBasicOrAnyType(string abletonType)
        {
            if (BasicTypeMap.TryGetValue(abletonType, out var mappedType))
            {
                return mappedType;
            }

            // Unknown => default to anyType
            return new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
        }

        /// <summary>
        /// Saves the compiled XSD to string.
        /// </summary>
        public string SaveToString()
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = true };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                _schema.Write(writer);
            }

            return sb.ToString();
        }
    }
}