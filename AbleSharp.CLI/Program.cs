using System.Xml.Linq;
using AbleSharp.Lib;
using AbleSharp.Lib.Schema;
using AbleSharp.Lib.Schema.Migrations;
using AbleSharp.SDK;
using AbleSharp.SDK.Options;
using AbleSharp.Schema.Generation;

namespace AbleSharp.CLI;

internal class Program
{
    private static readonly AbleSharpSdk _sdk = AbleSharpSdk.Instance;
    private static SchemaRepositoryManager? _schemaManager;

    private static SchemaRepositoryManager GetSchemaManager()
    {
        if (_schemaManager != null)
        {
            return _schemaManager;
        }

        var watchEnabled = string.Equals(Environment.GetEnvironmentVariable("ABLESHARP_SCHEMA_WATCH"), "1", StringComparison.OrdinalIgnoreCase);
        var managerOptions = new SchemaRepositoryManagerOptions
        {
            WatchForChanges = watchEnabled,
            Log = watchEnabled ? message => Console.WriteLine($"[schema] {message}") : null
        };

        _schemaManager = new SchemaRepositoryManager(null, managerOptions);
        return _schemaManager;
    }

    private static SchemaRepository GetSchemaRepository() => GetSchemaManager().Current;

    private static void ConfigureSchemaSources(IEnumerable<string> roots, IEnumerable<string> manifests)
    {
        var manager = GetSchemaManager();

        foreach (var root in roots)
        {
            try
            {
                manager.RegisterSchemaRoot(root);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: unable to register schema root '{root}': {ex.Message}");
            }
        }

        foreach (var manifest in manifests)
        {
            try
            {
                manager.RegisterManifest(manifest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: unable to register schema manifest '{manifest}': {ex.Message}");
            }
        }
    }

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "open":
                    HandleOpenCommand(args.Skip(1).ToArray());
                    break;

                case "create":
                    HandleCreateCommand(args.Skip(1).ToArray());
                    break;

                case "merge":
                    HandleMergeCommand(args.Skip(1).ToArray());
                    break;

                case "schema":
                    HandleSchemaCommand(args.Skip(1).ToArray());
                    break;

                case "schemas":
                    HandleSchemasCommand(args.Skip(1).ToArray());
                    break;

                case "migrate":
                    HandleMigrateCommand(args.Skip(1).ToArray());
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    break;
            }
        }
        catch (Exception? ex)
        {
            Console.WriteLine("An error occurred:");
            PrintExceptionDetails(ex);
            Environment.Exit(1);
        }
    }

    private static void PrintExceptionDetails(Exception? exception)
    {
        while (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
            Console.WriteLine($"Stack Trace: {exception.StackTrace}");
            exception = exception.InnerException;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("AbleSharp CLI Usage:");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  open [project file]                   Opens and validates a project file");
        Console.WriteLine("  open -d/--dump [project file]        Opens and dumps project info to a file");
        Console.WriteLine("  create [file path]                   Creates a new empty project");
        Console.WriteLine("  merge -o [output] [file1] [file2]... Merges multiple project files");
        Console.WriteLine("  schema [input schema] [output dir]    Generates C# classes from schema file");
        Console.WriteLine("  schemas list [options]               Lists bundled schema descriptors");
        Console.WriteLine("  migrate [options] <input>            Migrates project metadata between schemas");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ablesharp open project.als");
        Console.WriteLine("  ablesharp open --dump project.als");
        Console.WriteLine("  ablesharp create new_project.als");
        Console.WriteLine("  ablesharp merge -o merged.als project1.als project2.als");
        Console.WriteLine("  ablesharp schema 12.0_12203.txt GeneratedSchema/");
        Console.WriteLine("  ablesharp schemas list --major 12 --validate");
        Console.WriteLine("  ablesharp migrate project.xml --to 12.0_12203 --output migrated.xml");
        Console.WriteLine();
    }

    private static void PrintMigrateUsage()
    {
        Console.WriteLine("Usage: ablesharp migrate <input xml> [--from <version>] [--to <version>] [--output <path>] [--dry-run] [--plan]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --from, -f    Source schema version (auto-detected when omitted)");
        Console.WriteLine("  --to, -t      Target schema version (defaults to canonical)");
        Console.WriteLine("  --output, -o  Destination file path (defaults to <input>.<version>.xml)");
        Console.WriteLine("  --dry-run     Display the plan without writing output");
        Console.WriteLine("  --plan        Display the plan before executing");
        Console.WriteLine("  --schema-root <path>   Register an additional schema directory");
        Console.WriteLine("  --manifest <path>      Register an additional schema manifest");
        Console.WriteLine("  --telemetry            Emit migration telemetry to the console (opt-in)");
        Console.WriteLine();
    }

    private static void HandleOpenCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: No project file specified");
            return;
        }

        var shouldDump = false;
        var projectPath = "";

        // Parse arguments
        for (var i = 0; i < args.Length; i++)
            if (args[i] == "-d" || args[i] == "--dump")
                shouldDump = true;
            else
                projectPath = args[i];

        if (string.IsNullOrEmpty(projectPath))
        {
            Console.WriteLine("Error: No project file specified");
            return;
        }

        if (!File.Exists(projectPath))
        {
            Console.WriteLine($"Error: File not found: {projectPath}");
            return;
        }

        // Create options with logging
        var options = new ProjectOpenOptions
        {
            ErrorHandling = ErrorHandling.ThrowException,
            Logger = msg => Console.WriteLine($"SDK: {msg}")
        };

        // Load the project
        Console.WriteLine($"Opening project: {projectPath}");
        var project = _sdk.OpenProject(projectPath, options);
        Console.WriteLine("Project loaded successfully.");

        // Handle dump if requested
        if (shouldDump)
        {
            var dumpFileName = Path.GetFileName(projectPath) + ".dump.txt";
            var dumpDir = Path.GetDirectoryName(projectPath) ?? AppDomain.CurrentDomain.BaseDirectory;
            var dumpPath = Path.Combine(dumpDir, dumpFileName);

            var dumpText = _sdk.GetProjectDump(project);
            File.WriteAllText(dumpPath, dumpText);
            Console.WriteLine($"Project dump written to: {dumpPath}");
        }
    }

    private static void HandleCreateCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: No output path specified");
            return;
        }

        var outputPath = args[0];

        var options = new ProjectCreationOptions
        {
            CreateDefaultTracks = true,
            Logger = msg => Console.WriteLine($"SDK: {msg}")
        };

        // Create empty project
        Console.WriteLine("Creating new project...");
        var project = _sdk.CreateProject(options);

        // Save it
        var saveOptions = new ProjectSaveOptions
        {
            Compress = true,
            CreateBackup = true,
            Logger = options.Logger
        };

        Console.WriteLine($"Saving project to: {outputPath}");
        _sdk.SaveProject(project, outputPath, saveOptions);
        Console.WriteLine("Project created successfully.");
    }

    private static void HandleMergeCommand(string[] args)
    {
        if (args.Length < 3) // Need at least: -o output.als input1.als
        {
            Console.WriteLine("Error: Insufficient arguments for merge command");
            Console.WriteLine("Usage: ablesharp merge -o [output] [file1] [file2]...");
            return;
        }

        // Parse arguments
        var index = 0;
        var outputPath = "";
        var inputPaths = new List<string>();

        while (index < args.Length)
            if (args[index] == "-o")
            {
                if (index + 1 < args.Length)
                {
                    outputPath = args[index + 1];
                    index += 2;
                }
                else
                {
                    Console.WriteLine("Error: -o option requires an output path");
                    return;
                }
            }
            else
            {
                inputPaths.Add(args[index]);
                index++;
            }

        if (string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine("Error: No output path specified (use -o flag)");
            return;
        }

        if (inputPaths.Count < 1)
        {
            Console.WriteLine("Error: No input files specified");
            return;
        }

        // Validate input files exist
        foreach (var path in inputPaths)
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error: Input file not found: {path}");
                return;
            }

        var openOptions = new ProjectOpenOptions
        {
            Logger = msg => Console.WriteLine($"SDK: {msg}")
        };

        // Load all projects
        Console.WriteLine("Loading projects...");
        var projects = new List<AbletonProject>();
        foreach (var path in inputPaths)
        {
            Console.WriteLine($"Loading: {path}");
            var project = _sdk.OpenProject(path, openOptions);
            projects.Add(project);
        }

        // Merge projects with options
        var mergeOptions = new ProjectMergeOptions
        {
            PreserveColors = true,
            MergeScenes = true,
            NamingConflicts = ConflictResolution.Rename,
            Logger = msg => Console.WriteLine($"SDK: {msg}")
        };

        Console.WriteLine("Merging projects...");
        var mergedProject = _sdk.MergeProjects(projects, mergeOptions);

        // Save with options
        var saveOptions = new ProjectSaveOptions
        {
            CreateBackup = true,
            Compress = true,
            Logger = msg => Console.WriteLine($"SDK: {msg}")
        };

        Console.WriteLine($"Saving merged project to: {outputPath}");
        _sdk.SaveProject(mergedProject, outputPath, saveOptions);
        Console.WriteLine("Merge completed successfully.");
    }

    private static void HandleSchemaCommand(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Schema command requires input schema file or directory and output directory");
            Console.WriteLine("Usage: ablesharp schema [input schema or directory] [output dir]");
            return;
        }

        var schemaPath = args[0];
        var outputDir = args[1];

        // Ensure output directory exists
        Directory.CreateDirectory(outputDir);

        if (File.Exists(schemaPath))
        {
            // Single file processing
            ProcessSingleSchemaFile(schemaPath, outputDir);
        }
        else if (Directory.Exists(schemaPath))
        {
            // Directory processing
            string[] schemaFiles = Directory.GetFiles(schemaPath, "*.txt");

            if (schemaFiles.Length == 0)
            {
                Console.WriteLine($"No .txt files found in directory: {schemaPath}");
                return;
            }

            Console.WriteLine($"Processing {schemaFiles.Length} schema files from directory: {schemaPath}");
            foreach (var file in schemaFiles)
                try
                {
                    ProcessSingleSchemaFile(file, outputDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process schema file: {file}");
                    Console.WriteLine($"Error: {ex.Message}");
                }
        }
        else
        {
            Console.WriteLine($"Error: Schema file or directory not found: {schemaPath}");
        }
    }

    private static void HandleSchemasCommand(string[] args)
    {
        if (args.Length == 0)
        {
            PrintSchemasUsage();
            return;
        }

        var subcommand = args[0].ToLowerInvariant();

        switch (subcommand)
        {
            case "list":
                HandleSchemasList(args.Skip(1).ToArray());
                break;

            default:
                Console.WriteLine($"Unknown schemas subcommand: {subcommand}");
                PrintSchemasUsage();
                break;
        }
    }

    private static void HandleSchemasList(string[] args)
    {
        int? majorFilter = null;
        string? versionFilter = null;
        string? containsFilter = null;
        bool validate = false;
        bool showPath = false;
        var additionalRoots = new List<string>();
        var additionalManifests = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            var option = args[i];

            switch (option)
            {
                case "--major":
                case "-m":
                    if (i + 1 >= args.Length || !int.TryParse(args[++i], out var majorValue))
                    {
                        Console.WriteLine("Error: --major requires an integer value.");
                        return;
                    }

                    majorFilter = majorValue;
                    break;

                case "--version":
                case "-v":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --version requires a version identifier (e.g. 12.0_12203).");
                        return;
                    }

                    versionFilter = args[++i];
                    break;

                case "--contains":
                case "-c":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --contains requires a search string.");
                        return;
                    }

                    containsFilter = args[++i];
                    break;

                case "--validate":
                    validate = true;
                    break;

                case "--path":
                    showPath = true;
                    break;

                case "--schema-root":
                case "--root":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --schema-root requires a directory path.");
                        return;
                    }

                    additionalRoots.Add(args[++i]);
                    break;

                case "--manifest":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --manifest requires a file path.");
                        return;
                    }

                    additionalManifests.Add(args[++i]);
                    break;

                default:
                    Console.WriteLine($"Unknown option: {option}");
                    PrintSchemasUsage();
                    return;
            }
        }

        ConfigureSchemaSources(additionalRoots, additionalManifests);

        SchemaRepository repository;

        try
        {
            repository = GetSchemaRepository();
        }
        catch
        {
            return;
        }

        var descriptors = repository.GetDescriptors();
        var filtered = descriptors
            .Where(d => !majorFilter.HasValue || d.MajorVersion == majorFilter)
            .Where(d => versionFilter == null || string.Equals(d.VersionId, versionFilter, StringComparison.OrdinalIgnoreCase))
            .Where(d => containsFilter == null || d.VersionId.Contains(containsFilter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(d => d.VersionId, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (filtered.Count == 0)
        {
            Console.WriteLine("No schemas matched the provided filters.");
            return;
        }

        Console.WriteLine($"Found {filtered.Count} schema(s).");

        var nameColumn = showPath ? "Path" : "File";
        Console.WriteLine($"{"Version",-12} {"Major",5} {"Minor",5} {"Change",7} {"SizeKB",10} {"Status",-12} {nameColumn}");

        foreach (var descriptor in filtered)
        {
            SchemaValidationResult? validation = null;
            if (validate)
            {
                validation = repository.ValidateDescriptor(descriptor);
            }

            var status = validation switch
            {
                { IsValid: true } => "OK",
                { IsValid: false } result => result.Status.ToString(),
                _ => descriptor.FullPath != null ? "Cached" : "Missing"
            };

            var sizeInKb = descriptor.SizeBytes / 1024d;
            var label = showPath ? descriptor.FullPath ?? "<missing>" : descriptor.FileName;

            Console.WriteLine(
                $"{descriptor.VersionId,-12} {descriptor.MajorVersion,5} {descriptor.MinorVersion,5} {descriptor.SchemaChangeCount,7} {sizeInKb,10:F1} {status,-12} {label}");

            if (validation is { IsValid: false } invalid)
            {
                Console.WriteLine($"    -> {invalid.Message}");
            }
        }
    }

    private static void PrintSchemasUsage()
    {
        Console.WriteLine("Schemas command usage:");
        Console.WriteLine("  ablesharp schemas list [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  -m, --major <major>         Filter by major version (e.g. 12)");
        Console.WriteLine("  -v, --version <version>     Filter by exact version ID (e.g. 12.0_12203)");
        Console.WriteLine("  -c, --contains <text>       Filter versions containing the provided text");
        Console.WriteLine("      --validate              Verify descriptor hashes and file sizes");
        Console.WriteLine("      --path                  Show the resolved schema file path");
        Console.WriteLine("      --schema-root <path>    Register an additional schema directory (repeatable)");
        Console.WriteLine("      --manifest <path>       Register an additional schema manifest (repeatable)");
    }

    private static void ProcessSingleSchemaFile(string schemaPath, string outputDir)
    {
        // Extract the minor version string by taking the file name without extension
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(schemaPath);
        var sanitizedVersion = "v" + fileNameWithoutExtension.Replace('.', '_');

        // Generate output path
        var outputFile = Path.Combine(outputDir, $"{sanitizedVersion}.cs");

        // Skip processing if the output file already exists
        if (File.Exists(outputFile))
        {
            Console.WriteLine($"Skipping file as output already exists: {outputFile}");
            return;
        }

        try
        {
            Console.WriteLine($"Reading schema from: {schemaPath}");
            var schemaContent = File.ReadAllText(schemaPath);

            Console.WriteLine("Generating C# classes...");
            var emitter = new SchemaCodeEmitter();
            var target = new SchemaGenerationTarget(
                fileNameWithoutExtension,
                fileNameWithoutExtension,
                sanitizedVersion,
                Path.GetFileName(schemaPath));
            var source = emitter.Generate(SchemaCatalog.SchemaNamespaceRoot, target, schemaContent);

            File.WriteAllText(outputFile, source);

            Console.WriteLine($"Classes generated successfully at: {outputFile}");
        }
        catch (Exception? ex)
        {
            Console.WriteLine($"Error generating classes from schema file: {schemaPath}");
            PrintExceptionDetails(ex);
            throw; // Rethrow to allow the caller to handle it
        }
    }

    private static void HandleMigrateCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: migrate command requires an input file.");
            PrintMigrateUsage();
            return;
        }

        string? inputPath = null;
        string? fromVersion = null;
        string? toVersion = null;
        string? outputPath = null;
        var dryRun = false;
        var showPlan = false;
        var additionalRoots = new List<string>();
        var additionalManifests = new List<string>();
        var telemetryEnabled = string.Equals(Environment.GetEnvironmentVariable("ABLESHARP_MIGRATION_TELEMETRY"), "1", StringComparison.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];

            switch (current)
            {
                case "--from":
                case "-f":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --from option requires a value.");
                        return;
                    }

                    fromVersion = args[++i];
                    break;

                case "--to":
                case "-t":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --to option requires a value.");
                        return;
                    }

                    toVersion = args[++i];
                    break;

                case "--output":
                case "-o":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --output option requires a value.");
                        return;
                    }

                    outputPath = args[++i];
                    break;

                case "--dry-run":
                    dryRun = true;
                    break;

                case "--plan":
                    showPlan = true;
                    break;

                case "--schema-root":
                case "--root":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --schema-root option requires a directory path.");
                        return;
                    }

                    additionalRoots.Add(args[++i]);
                    break;

                case "--manifest":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Error: --manifest option requires a file path.");
                        return;
                    }

                    additionalManifests.Add(args[++i]);
                    break;

                case "--telemetry":
                    telemetryEnabled = true;
                    break;

                default:
                    if (current.StartsWith("-", StringComparison.Ordinal))
                    {
                        Console.WriteLine($"Unknown option: {current}");
                        PrintMigrateUsage();
                        return;
                    }

                    inputPath ??= current;
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(inputPath))
        {
            Console.WriteLine("Error: Input file path is required.");
            PrintMigrateUsage();
            return;
        }

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Error: Input file not found: {inputPath}");
            return;
        }

        ConfigureSchemaSources(additionalRoots, additionalManifests);

        var repository = GetSchemaRepository();
        var detector = new AbleSharp.Lib.Schema.SchemaVersionDetector(repository);

        if (string.IsNullOrWhiteSpace(fromVersion))
        {
            using var stream = File.OpenRead(inputPath);
            var resolution = detector.DetectVersion(stream);
            fromVersion = resolution.VersionId;
            Console.WriteLine($"Detected source version: {fromVersion}");
        }

        toVersion ??= SchemaCatalog.CanonicalVersionId;

        try
        {
            repository.EnsureDescriptor(toVersion!);
        }
        catch (Exception)
        {
            Console.WriteLine($"Error: Target version '{toVersion}' is not registered.");
            return;
        }

        try
        {
            repository.EnsureDescriptor(fromVersion!);
        }
        catch (Exception)
        {
            Console.WriteLine($"Error: Source version '{fromVersion}' is not registered.");
            return;
        }

        var telemetry = telemetryEnabled ? new ConsoleMigrationTelemetry() : MigrationTelemetry.Disabled;
        var engine = new MigrationEngine(repository, DefaultSchemaMigrations.All, telemetry);
        var direction = SchemaMigrationDirection.Forward;
        SchemaMigrationPlan plan;

        try
        {
            plan = engine.Plan(fromVersion!, toVersion!, direction);
        }
        catch (SchemaMigrationException)
        {
            direction = SchemaMigrationDirection.Backward;

            try
            {
                plan = engine.Plan(fromVersion!, toVersion!, direction);
            }
            catch (SchemaMigrationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        if (plan.IsIdentity)
        {
            Console.WriteLine("Source and target versions match; no migration required.");
            return;
        }

        if (showPlan || dryRun)
        {
            PrintMigrationPlan(plan);
        }

        if (dryRun)
        {
            Console.WriteLine("Dry-run requested. No changes were written.");
            return;
        }

        XDocument migrated;

        try
        {
            var document = XDocument.Load(inputPath, LoadOptions.PreserveWhitespace);
            migrated = engine.Execute(plan, document, new MigrationContext(repository));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            return;
        }

        outputPath ??= GetDefaultMigrationOutputPath(inputPath, toVersion!);
        migrated.Save(outputPath);

        Console.WriteLine($"Migration complete. Written to {outputPath}");
    }

    private static void PrintMigrationPlan(SchemaMigrationPlan plan)
    {
        Console.WriteLine($"Migration plan ({plan.Direction}): {plan.SourceVersionId} -> {plan.TargetVersionId}");

        foreach (var step in plan.Steps)
        {
            var metadata = step.Metadata;
            Console.WriteLine($"  - {metadata.SourceVersionId} -> {metadata.TargetVersionId}: {metadata.Description}");
        }
    }

    private static string GetDefaultMigrationOutputPath(string inputPath, string targetVersion)
    {
        var directory = Path.GetDirectoryName(inputPath) ?? string.Empty;
        var name = Path.GetFileNameWithoutExtension(inputPath);
        var extension = Path.GetExtension(inputPath);
        var suffix = string.IsNullOrWhiteSpace(extension) ? ".xml" : extension;

        return Path.Combine(directory, $"{name}.{targetVersion}{suffix}");
    }

    private sealed class ConsoleMigrationTelemetry : IMigrationTelemetry
    {
        public void OnPlanCreated(SchemaMigrationPlan plan)
        {
            Console.WriteLine($"[telemetry] Planned {plan.Direction} migration: {plan.SourceVersionId} -> {plan.TargetVersionId} ({plan.Steps.Count} step(s))");
        }

        public void OnStepExecuting(SchemaMigrationPlan plan, ISchemaMigration migration, int index, int total)
        {
            Console.WriteLine($"[telemetry] Executing step {index + 1}/{total}: {migration.Metadata.Description}");
        }

        public void OnStepSucceeded(SchemaMigrationPlan plan, ISchemaMigration migration, TimeSpan duration, int index, int total)
        {
            Console.WriteLine($"[telemetry] Completed step {index + 1}/{total} in {duration.TotalMilliseconds:F0} ms");
        }

        public void OnPlanSucceeded(SchemaMigrationPlan plan, TimeSpan totalDuration)
        {
            Console.WriteLine($"[telemetry] Migration succeeded in {totalDuration.TotalMilliseconds:F0} ms");
        }

        public void OnPlanFailed(SchemaMigrationPlan plan, ISchemaMigration? migration, Exception exception)
        {
            var stepDescription = migration?.Metadata.Description ?? "unknown";
            Console.WriteLine($"[telemetry] Migration failed during '{stepDescription}': {exception.Message}");
        }
    }
}