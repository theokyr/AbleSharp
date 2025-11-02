using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AbleSharp.Lib.Schema;

/// <summary>
/// Detects Ableton Live schema metadata and resolves descriptors through <see cref="SchemaRepository"/>.
/// </summary>
public sealed class SchemaVersionDetector
{
    private static readonly Regex VersionRegex = new(
        "<Ableton\\s+MajorVersion=\"(?<major>\\d+)\"\\s+MinorVersion=\"(?<minor>[^\"]+)\"\\s+SchemaChangeCount=\"(?<change>\\d+)\"",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly SchemaRepository _repository;

    /// <summary>
    /// Gets the underlying schema repository.
    /// </summary>
    public SchemaRepository Repository => _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaVersionDetector"/> class.
    /// </summary>
    /// <param name="repository">The schema repository used to resolve descriptors.</param>
    public SchemaVersionDetector(SchemaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Detects the schema version information contained in the Ableton project stream.
    /// </summary>
    /// <param name="stream">A seekable stream positioned at the beginning of the project XML.</param>
    /// <param name="requireDescriptor">Throws when set to <c>true</c> and the version is not registered.</param>
    /// <param name="validateIntegrity">When <c>true</c>, validates the descriptor before returning.</param>
    /// <returns>The discovered version information.</returns>
    /// <exception cref="ArgumentException">Thrown when the stream is not seekable.</exception>
    /// <exception cref="InvalidOperationException">Thrown when version metadata is missing.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when <paramref name="requireDescriptor"/> is <c>true</c> and the version is unknown.</exception>
    public SchemaVersionResolution DetectVersion(Stream stream, bool requireDescriptor = false, bool validateIntegrity = false)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream must support seeking for version detection.", nameof(stream));
        }

        var initialPosition = stream.Position;

        try
        {
            var headerSnippet = ReadHeaderSnippet(stream);
            var match = VersionRegex.Match(headerSnippet);

            if (!match.Success)
            {
                throw new InvalidOperationException("Could not detect Ableton schema version in project header.");
            }

            var majorVersion = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);
            var minorVersion = match.Groups["minor"].Value;
            var schemaChangeCount = int.Parse(match.Groups["change"].Value, CultureInfo.InvariantCulture);
            var versionId = $"{majorVersion}.{minorVersion}_{schemaChangeCount}";

            SchemaDescriptor? descriptor = null;

            if (_repository.TryGetDescriptor(versionId, out var candidate))
            {
                if (validateIntegrity)
                {
                    var validation = _repository.ValidateDescriptor(candidate);
                    if (!validation.IsValid)
                    {
                        throw new InvalidOperationException(validation.Message);
                    }
                }

                descriptor = candidate;
            }
            else if (requireDescriptor)
            {
                throw new KeyNotFoundException($"Schema descriptor '{versionId}' was not found. Ensure the schema catalog is up to date.");
            }

            return new SchemaVersionResolution(majorVersion, minorVersion, schemaChangeCount, descriptor);
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = initialPosition;
            }
        }
    }

    private static string ReadHeaderSnippet(Stream stream)
    {
        var builder = new StringBuilder(capacity: 512);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 512, leaveOpen: true);

        for (var i = 0; i < 4; i++)
        {
            var line = reader.ReadLine();
            if (line is null)
            {
                break;
            }

            builder.Append(line);
        }

        return builder.ToString();
    }
}

