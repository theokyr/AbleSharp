using System.Text.Json.Serialization;

namespace AbleSharp.Lib.Schema;

/// <summary>
/// Describes a single Ableton schema resource bundled with AbleSharp.
/// </summary>
public sealed class SchemaDescriptor
{
    /// <summary>
    /// Gets the canonical Ableton version identifier (e.g. 12.0_12203).
    /// </summary>
    public required string VersionId { get; init; }

    /// <summary>
    /// Gets the major version of the Ableton schema.
    /// </summary>
    public required int MajorVersion { get; init; }

    /// <summary>
    /// Gets the minor version string of the Ableton schema.
    /// </summary>
    public required string MinorVersion { get; init; }

    /// <summary>
    /// Gets the schema change counter extracted from the Ableton header.
    /// </summary>
    public required int SchemaChangeCount { get; init; }

    /// <summary>
    /// Gets the schema file name relative to the schema root directory.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the expected file size in bytes.
    /// </summary>
    public required long SizeBytes { get; init; }

    /// <summary>
    /// Gets the expected SHA-256 checksum of the schema asset (lowercase hex).
    /// </summary>
    public required string Sha256 { get; init; }

    /// <summary>
    /// Gets the UTC timestamp of the underlying file captured during catalog generation.
    /// </summary>
    public DateTimeOffset LastModified { get; init; }

    /// <summary>
    /// Gets the resolved absolute schema path if available.
    /// </summary>
    [JsonIgnore]
    public string? FullPath { get; internal set; }
}

