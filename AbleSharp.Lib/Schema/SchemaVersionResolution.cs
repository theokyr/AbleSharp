namespace AbleSharp.Lib.Schema;

/// <summary>
/// Represents the outcome of inspecting an Ableton project for schema metadata.
/// </summary>
/// <param name="MajorVersion">The major Ableton version discovered in the project header.</param>
/// <param name="MinorVersion">The minor Ableton version discovered in the project header.</param>
/// <param name="SchemaChangeCount">The schema change counter extracted from the header.</param>
/// <param name="Descriptor">The repository descriptor if one was found for the version.</param>
public sealed record SchemaVersionResolution(
    int MajorVersion,
    string MinorVersion,
    int SchemaChangeCount,
    SchemaDescriptor? Descriptor)
{
    /// <summary>
    /// Gets the normalized Ableton schema version identifier (e.g. 12.0_12203).
    /// </summary>
    public string VersionId => $"{MajorVersion}.{MinorVersion}_{SchemaChangeCount}";

    /// <summary>
    /// Gets a value indicating whether a descriptor was located for the version.
    /// </summary>
    public bool IsKnown => Descriptor is not null;
}

