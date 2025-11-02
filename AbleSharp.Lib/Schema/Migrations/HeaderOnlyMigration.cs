using System.Xml.Linq;

namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Migration that updates Ableton header metadata without touching body content.
/// </summary>
internal sealed class HeaderOnlyMigration : ISchemaMigration
{
    private readonly SchemaMigrationMetadata _metadata;
    private readonly Action<XElement>? _headerMutator;

    private HeaderOnlyMigration(SchemaMigrationMetadata metadata, Action<XElement>? headerMutator)
    {
        _metadata = metadata;
        _headerMutator = headerMutator;
    }

    public SchemaMigrationMetadata Metadata => _metadata;

    public XDocument Apply(XDocument document, MigrationContext context)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var clone = new XDocument(document);
        var root = clone.Root ?? throw new SchemaMigrationException("Document does not contain an <Ableton> root element.");

        root.SetAttributeValue("MinorVersion", _metadata.TargetVersionId);
        _headerMutator?.Invoke(root);

        return clone;
    }

    public static HeaderOnlyMigration Forward(string sourceVersionId, string targetVersionId, string description, Action<XElement>? headerMutator = null)
    {
        return new HeaderOnlyMigration(
            new SchemaMigrationMetadata(sourceVersionId, targetVersionId, SchemaMigrationDirection.Forward, description),
            headerMutator);
    }

    public static HeaderOnlyMigration Backward(string sourceVersionId, string targetVersionId, string description, Action<XElement>? headerMutator = null)
    {
        return new HeaderOnlyMigration(
            new SchemaMigrationMetadata(sourceVersionId, targetVersionId, SchemaMigrationDirection.Backward, description),
            headerMutator);
    }
}

