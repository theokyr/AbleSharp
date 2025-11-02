using System.IO;
using System.Linq;
using NUnit.Framework;
using AbleSharp.Schema;

namespace AbleSharp.Schema.Tests
{
    [TestFixture]
    public class SchemaConverterTests
    {
        private SchemaConverterService _service;
        private string _schemaPath = null!;
        private string _projectPath = null!;

        [SetUp]
        public void Setup()
        {
            _service = new SchemaConverterService();
            _schemaPath = ResolvePath("resources", "schemas", "12.0_12203.txt");
            _projectPath = ResolvePath("extras", "ableton_example_project.xml");
        }

        [Test]
        public void ConvertSchema_ValidFileInput_GeneratesValidXsd()
        {
            // Act
            var xsdSchema = _service.ConvertSchemaFile(_schemaPath);

            // Assert
            Assert.That(xsdSchema, Is.Not.Null);
            Assert.That(xsdSchema, Does.Contain("xs:schema"));
            Assert.That(xsdSchema, Does.Contain("LiveSet"));
            Assert.That(xsdSchema, Does.Contain("Track"));
            Assert.That(xsdSchema, Does.Contain("element name=\"Ableton\""));
        }

        [Test]
        public void ValidateAgainstSchema_ValidAbletonProject_ReturnsTrue()
        {
            // Arrange
            var xsdSchema = _service.ConvertSchemaFile(_schemaPath);

            // Act
            var isValid = _service.ValidateAgainstSchema(_projectPath, xsdSchema);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void ValidateAgainstSchema_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var xsdSchema = _service.ConvertSchemaFile(_schemaPath);
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => 
                _service.ValidateAgainstSchema(nonExistentPath, xsdSchema));
        }

        [Test]
        public void ConvertSchema_NonExistentSchemaFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => 
                _service.ConvertSchemaFile(nonExistentPath));
        }

        // [Test]
        // [Test]
        // public void ValidateAgainstSchema_InvalidXmlContent_ReturnsFalse()
        // {
        //     // Arrange
        //     var loggerFactory = LoggerFactory.Create(builder => 
        //         builder.AddConsole()
        //             .AddDebug()
        //             .SetMinimumLevel(LogLevel.Debug));
        //
        //     var logger = loggerFactory.CreateLogger<SchemaConverterTests>();
        //
        //     var xsdSchema = _service.ConvertSchemaFile(_schemaPath);
        //     var invalidXmlPath = Path.GetTempFileName();
        //     File.WriteAllText(invalidXmlPath, "<Invalid>XML</Invalid>");
        //
        //     try
        //     {
        //         // Act
        //         var isValid = _service.ValidateAgainstSchema(invalidXmlPath, xsdSchema, logger);
        //
        //         // Assert
        //         Assert.That(isValid, Is.False);
        //     }
        //     finally
        //     {
        //         File.Delete(invalidXmlPath);
        //     }
        // }
        private static string ResolvePath(params string[] segments)
        {
            var root = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", ".."));
            return Path.Combine(new[] { root }.Concat(segments).ToArray());
        }
    }
}