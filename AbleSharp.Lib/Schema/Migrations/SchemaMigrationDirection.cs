namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Indicates whether a schema migration moves forward (towards newer schemas) or backward (towards older schemas).
/// </summary>
public enum SchemaMigrationDirection
{
    Forward,
    Backward
}

