using AbleSharp.Lib;
using AbleSharp.SDK;
using AbleSharp.SDK.Options;

namespace AbleSharp.CLI;

internal class Program
{
    private static readonly AbleSharpSdk _sdk = AbleSharpSdk.Instance;

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
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ablesharp open project.als");
        Console.WriteLine("  ablesharp open --dump project.als");
        Console.WriteLine("  ablesharp create new_project.als");
        Console.WriteLine("  ablesharp merge -o merged.als project1.als project2.als");
        Console.WriteLine("  ablesharp schema 12.0_12049.txt GeneratedSchema/");
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
            var generator = new SchemaGenerator();

            generator.GenerateForMinorVersion(fileNameWithoutExtension, schemaContent, outputFile);

            Console.WriteLine($"Classes generated successfully at: {outputFile}");
        }
        catch (Exception? ex)
        {
            Console.WriteLine($"Error generating classes from schema file: {schemaPath}");
            PrintExceptionDetails(ex);
            throw; // Rethrow to allow the caller to handle it
        }
    }
}