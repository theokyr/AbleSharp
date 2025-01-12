using AbleSharp.SDK;

namespace AbleSharp.CLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: AbleSharp.CLI <als file path>");
            return;
        }

        string alsFilePath = args[0];

        if (!File.Exists(alsFilePath))
        {
            Console.WriteLine($"Error: The file '{alsFilePath}' does not exist.");
            return;
        }

        var existingSet = AbletonProjectHandler.LoadFromFile(alsFilePath);

        var dumpText = AbletonProjectDumper.DebugDumpProject(existingSet);

        string alsFileName = Path.GetFileName(alsFilePath);
        string dumpFileName = alsFileName + ".dump.txt";

        string dumpFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dumpFileName);

        File.WriteAllText(dumpFilePath, dumpText);

        Console.WriteLine($"Dump successfully written to: {dumpFilePath}");
    }
}