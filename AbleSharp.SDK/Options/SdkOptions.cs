namespace AbleSharp.SDK;

/// <summary>
/// Options for SDK operations
/// </summary>
public class SdkOptions
{
    /// <summary>
    /// How to handle errors during operations
    /// </summary>
    public ErrorHandling ErrorHandling { get; set; } = ErrorHandling.ThrowException;

    /// <summary>
    /// Whether to create backups before overwriting files
    /// </summary>
    public bool CreateBackups { get; set; } = true;

    /// <summary>
    /// Optional logging callback for operations
    /// </summary>
    public Action<string>? Logger { get; set; }
}

