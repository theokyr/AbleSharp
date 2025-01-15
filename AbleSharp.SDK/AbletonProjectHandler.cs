using AbleSharp.SDK.Factories;
using AbleSharp.SDK.Handlers;

namespace AbleSharp.SDK;

using Lib;

public static class AbletonProjectHandler
{
    public static AbletonProject LoadFromFile(string filePath)
    {
        return AbletonProjectReader.LoadFromFile(filePath);
    }

    public static void SaveToFile(AbletonProject project, string filePath)
    {
        AbletonProjectWriter.SaveToFile(project, filePath);
    }

    public static AbletonProject CreateBlankProject()
    {
        return AbletonProjectFactory.CreateBlankProject();
    }
}