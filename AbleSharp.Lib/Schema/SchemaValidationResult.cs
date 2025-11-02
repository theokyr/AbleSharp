namespace AbleSharp.Lib.Schema;

/// <summary>
/// Represents the result of validating a schema file against its descriptor metadata.
/// </summary>
public readonly record struct SchemaValidationResult(
    SchemaDescriptor Descriptor,
    SchemaValidationStatus Status,
    string Message,
    long? ActualSize = null,
    string? ActualSha256 = null)
{
    /// <summary>
    /// Gets a value indicating whether the validation succeeded.
    /// </summary>
    public bool IsValid => Status == SchemaValidationStatus.Valid;

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static SchemaValidationResult Valid(SchemaDescriptor descriptor) =>
        new(descriptor, SchemaValidationStatus.Valid, $"Schema '{descriptor.VersionId}' validated successfully.");

    /// <summary>
    /// Creates a result describing a missing schema file.
    /// </summary>
    public static SchemaValidationResult Missing(SchemaDescriptor descriptor) =>
        new(descriptor, SchemaValidationStatus.MissingFile, $"Schema file '{descriptor.FileName}' was not found in any configured schema root.");

    /// <summary>
    /// Creates a result describing a size mismatch.
    /// </summary>
    public static SchemaValidationResult SizeMismatch(SchemaDescriptor descriptor, long actualSize) =>
        new(
            descriptor,
            SchemaValidationStatus.SizeMismatch,
            $"Schema file '{descriptor.FileName}' size mismatch. Expected {descriptor.SizeBytes} bytes but found {actualSize} bytes.",
            actualSize);

    /// <summary>
    /// Creates a result describing a checksum mismatch.
    /// </summary>
    public static SchemaValidationResult HashMismatch(SchemaDescriptor descriptor, string actualSha256, long actualSize) =>
        new(
            descriptor,
            SchemaValidationStatus.HashMismatch,
            $"Schema file '{descriptor.FileName}' checksum mismatch.",
            actualSize,
            actualSha256);
}

/// <summary>
/// Enumerates possible schema validation outcomes.
/// </summary>
public enum SchemaValidationStatus
{
    /// <summary>
    /// Validation succeeded and the schema can be used.
    /// </summary>
    Valid,

    /// <summary>
    /// The schema file could not be located under any configured root directory.
    /// </summary>
    MissingFile,

    /// <summary>
    /// The schema file was found but its size differs from the descriptor metadata.
    /// </summary>
    SizeMismatch,

    /// <summary>
    /// The schema file was found but its SHA-256 hash differs from the descriptor metadata.
    /// </summary>
    HashMismatch
}

