using AbleSharp.Lib;
using AbleSharp.SDK;

namespace AbleSharp.CLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();

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

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("AbleSharp CLI Usage:");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  open [project file]                   Opens and validates a project file");
        Console.WriteLine("  open -d/--dump [project file]        Opens and dumps project info to a file");
        Console.WriteLine("  create [file path]                   Creates a new empty project");
        Console.WriteLine("  merge -o [output] [file1] [file2]... Merges multiple project files");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ablesharp open project.als");
        Console.WriteLine("  ablesharp open --dump project.als");
        Console.WriteLine("  ablesharp create new_project.als");
        Console.WriteLine("  ablesharp merge -o merged.als project1.als project2.als");
    }

    static void HandleOpenCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: No project file specified");
            return;
        }

        bool shouldDump = false;
        string projectPath = "";

        // Parse arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-d" || args[i] == "--dump")
            {
                shouldDump = true;
            }
            else
            {
                projectPath = args[i];
            }
        }

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

        // Load the project
        Console.WriteLine($"Opening project: {projectPath}");
        var project = AbletonProjectHandler.LoadFromFile(projectPath);
        Console.WriteLine("Project loaded successfully.");

        // Handle dump if requested
        if (shouldDump)
        {
            string dumpFileName = Path.GetFileName(projectPath) + ".dump.txt";
            string dumpDir = Path.GetDirectoryName(projectPath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string dumpPath = Path.Combine(dumpDir, dumpFileName);

            var dumpText = AbletonProjectDumper.DebugDumpProject(project);
            File.WriteAllText(dumpPath, dumpText);
            Console.WriteLine($"Project dump written to: {dumpPath}");
        }
    }

    static void HandleCreateCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: No output path specified");
            return;
        }

        string outputPath = args[0];

        // Create empty project
        Console.WriteLine("Creating new project...");
        var project = AbletonProjectHandler.CreateBlankProject();

        // Save it
        Console.WriteLine($"Saving project to: {outputPath}");
        AbletonProjectHandler.SaveToFile(project, outputPath);
        Console.WriteLine("Project created successfully.");
    }

    static void HandleMergeCommand(string[] args)
    {
        if (args.Length < 3) // Need at least: -o output.als input1.als
        {
            Console.WriteLine("Error: Insufficient arguments for merge command");
            Console.WriteLine("Usage: ablesharp merge -o [output] [file1] [file2]...");
            return;
        }

        // Parse arguments
        int index = 0;
        string outputPath = "";
        var inputPaths = new List<string>();

        while (index < args.Length)
        {
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
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error: Input file not found: {path}");
                return;
            }
        }

        // Load all projects
        Console.WriteLine("Loading projects...");
        var projects = new List<AbletonProject>();
        foreach (var path in inputPaths)
        {
            Console.WriteLine($"Loading: {path}");
            var project = AbletonProjectHandler.LoadFromFile(path);
            projects.Add(project);
        }

        // Merge projects
        Console.WriteLine("Merging projects...");
        var mergedProject = AbletonProjectMerger.MergeProjects(projects);

        // Save result
        Console.WriteLine($"Saving merged project to: {outputPath}");
        AbletonProjectHandler.SaveToFile(mergedProject, outputPath);
        Console.WriteLine("Merge completed successfully.");
    }
}