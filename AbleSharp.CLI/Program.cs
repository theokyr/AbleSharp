using AbleSharp.SDK;

namespace AbleSharp.CLI;

class Program
{
    static void Main(string[] args)
    {
        // Create a new blank project and save it
        var newSet = AbletonProjectHandler.CreateBlankProject();
        AbletonProjectHandler.SaveToFile(newSet, "generated.als");
        Console.WriteLine("Created new blank project");

        // TODO: We don't care about loading existing files for now.
        // var existingSet = AbletonProjectHandler.LoadFromFile(ProjectPath);
        // Console.WriteLine($"Loaded project created by: {existingSet.Creator}");
    }
}