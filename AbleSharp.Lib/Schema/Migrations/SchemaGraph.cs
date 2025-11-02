namespace AbleSharp.Lib.Schema.Migrations;

/// <summary>
/// Represents the directed graph of registered schema migrations.
/// </summary>
internal sealed class SchemaGraph
{
    private readonly Dictionary<string, List<ISchemaMigration>> _forwardEdges;
    private readonly Dictionary<string, List<ISchemaMigration>> _backwardEdges;

    public SchemaGraph(IEnumerable<ISchemaMigration> migrations)
    {
        if (migrations is null)
        {
            throw new ArgumentNullException(nameof(migrations));
        }

        _forwardEdges = new Dictionary<string, List<ISchemaMigration>>(StringComparer.OrdinalIgnoreCase);
        _backwardEdges = new Dictionary<string, List<ISchemaMigration>>(StringComparer.OrdinalIgnoreCase);

        foreach (var migration in migrations)
        {
            var metadata = migration.Metadata;

            if (metadata.Direction == SchemaMigrationDirection.Forward)
            {
                AddEdge(_forwardEdges, metadata.SourceVersionId, migration);
            }
            else
            {
                AddEdge(_backwardEdges, metadata.SourceVersionId, migration);
            }
        }
    }

    public IReadOnlyList<ISchemaMigration> FindPath(string sourceVersionId, string targetVersionId, SchemaMigrationDirection direction)
    {
        if (string.IsNullOrWhiteSpace(sourceVersionId))
        {
            throw new ArgumentException("Source version identifier cannot be null or empty.", nameof(sourceVersionId));
        }

        if (string.IsNullOrWhiteSpace(targetVersionId))
        {
            throw new ArgumentException("Target version identifier cannot be null or empty.", nameof(targetVersionId));
        }

        if (string.Equals(sourceVersionId, targetVersionId, StringComparison.OrdinalIgnoreCase))
        {
            return Array.Empty<ISchemaMigration>();
        }

        var edges = direction == SchemaMigrationDirection.Forward ? _forwardEdges : _backwardEdges;
        var comparer = StringComparer.OrdinalIgnoreCase;
        var visited = new HashSet<string>(comparer) { sourceVersionId };
        var queue = new Queue<(string Node, List<ISchemaMigration> Path)>();
        queue.Enqueue((sourceVersionId, new List<ISchemaMigration>()));

        while (queue.Count > 0)
        {
            var (node, path) = queue.Dequeue();

            if (!edges.TryGetValue(node, out var outgoing))
            {
                continue;
            }

            foreach (var migration in outgoing)
            {
                var next = migration.Metadata.TargetVersionId;
                if (!visited.Add(next))
                {
                    continue;
                }

                var candidatePath = new List<ISchemaMigration>(path) { migration };
                if (comparer.Equals(next, targetVersionId))
                {
                    return candidatePath;
                }

                queue.Enqueue((next, candidatePath));
            }
        }

        return Array.Empty<ISchemaMigration>();
    }

    private static void AddEdge(Dictionary<string, List<ISchemaMigration>> graph, string key, ISchemaMigration migration)
    {
        if (!graph.TryGetValue(key, out var list))
        {
            list = new List<ISchemaMigration>();
            graph[key] = list;
        }

        list.Add(migration);
    }
}

