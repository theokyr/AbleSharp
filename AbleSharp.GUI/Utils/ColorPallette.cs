namespace AbleSharp.GUI;

public static class ColorPalette
{
    private static readonly Dictionary<int, string> ColorMap = new()
    {
        { 0, "#eca3b4" },
        { 1, "#d99f4f" },
        { 2, "#a18b52" },
        { 3, "#a18b52" },
        { 4, "#a4d42d" },
        { 5, "#35dc52" },
        { 6, "#3fdda8" },
        { 7, "#6fe6dc" },
        { 8, "#99c4f1" },
        { 9, "#6187d7" },
        { 10, "#9fb0f2" },
        { 11, "#bd7eda" },
        { 12, "#c269aa" },
        { 13, "#ffffff" },
        { 14, "#db535a" },
        { 15, "#c96f30" },
        { 16, "#756e6f" },
        { 17, "#dbd558" },
        { 18, "#8fe782" },
        { 19, "#42a523" },
        { 20, "#16a19d" },
        { 21, "#34cce4" },
        { 22, "#2896d6" },
        { 23, "#1773aa" },
        { 24, "#887eda" },
        { 25, "#9c83c8" },
        { 26, "#9c83c8" },
        { 27, "#c4cedc" },
        { 28, "#bf777d" },
        { 29, "#e6a98d" },
        { 30, "#b3a691" },
        { 31, "#e4f2bd" },
        { 32, "#c2d5af" },
        { 33, "#a3bc94" },
        { 34, "#90b6a8" },
        { 35, "#d9f6e5" },
        { 36, "#d1ecf4" },
        { 37, "#b9c4e3" },
        { 38, "#c6c1e4" },
        { 39, "#aba4e0" },
        { 40, "#dcdde8" },
        { 41, "#93a5bf" },
        { 42, "#aa98a7" },
        { 43, "#8f807e" },
        { 44, "#747d8f" },
        { 45, "#9ca88c" },
        { 46, "#8ba122" },
        { 47, "#679976" },
        { 48, "#82b3c3" },
        { 49, "#93aecc" },
        { 50, "#80a1c7" },
        { 51, "#8298cd" },
        { 52, "#9298c2" },
        { 53, "#a7a2c9" },
        { 54, "#9a7ca9" },
        { 55, "#5c759a" },
        { 56, "#88465a" },
        { 57, "#835857" },
        { 58, "#56505d" },
        { 59, "#b4a827" },
        { 60, "#6b7f3f" },
        { 61, "#4b8855" },
        { 62, "#1a8483" },
        { 63, "#285b7f" },
        { 64, "#253b8b" },
        { 65, "#34579d" },
        { 66, "#5659b0" },
        { 67, "#7e59b0" },
        { 68, "#a04684" },
        { 69, "#2d394b" },
    };


    public static string GetColor(int colorIndex)
    {
        if (!ColorMap.TryGetValue(colorIndex, out var color))
            return ColorMap[0];

        return color;
    }

    public static string GetLightColor(int colorIndex)
    {
        var baseColor = GetColor(colorIndex);
        return AdjustBrightness(baseColor, 0.7);
    }

    public static string GetDarkColor(int colorIndex)
    {
        var baseColor = GetColor(colorIndex);
        return AdjustBrightness(baseColor, -0.3);
    }

    private static string AdjustBrightness(string hexColor, double factor)
    {
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);

        if (hexColor.Length != 6)
            return hexColor;

        var r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        r = Clamp((int)(r + factor * 255));
        g = Clamp((int)(g + factor * 255));
        b = Clamp((int)(b + factor * 255));

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static int Clamp(int value)
    {
        return Math.Max(0, Math.Min(255, value));
    }
}