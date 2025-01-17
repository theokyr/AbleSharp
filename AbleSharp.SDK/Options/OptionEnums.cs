namespace AbleSharp.SDK;

/// <summary>
/// How the SDK should handle errors
/// </summary>
public enum ErrorHandling
{
    /// <summary>
    /// Throw exceptions normally (default)
    /// </summary>
    ThrowException,

    /// <summary>
    /// Return an empty project instead of throwing
    /// </summary>
    ReturnEmpty
}

/// <summary>
/// How to resolve naming conflicts
/// </summary>
public enum ConflictResolution
{
    /// <summary>
    /// Automatically rename conflicting items
    /// </summary>
    Rename,

    /// <summary>
    /// Keep original names, skip duplicates
    /// </summary>
    Skip,

    /// <summary>
    /// Cancel operation on conflict
    /// </summary>
    Cancel
}

/// <summary>
/// How to handle features not supported in target version
/// </summary>
public enum FeatureHandling
{
    /// <summary>
    /// Remove missing classes
    /// </summary>
    RemoveWithWarnings,

    /// <summary>
    /// Cancel operation 
    /// </summary>
    Cancel
}