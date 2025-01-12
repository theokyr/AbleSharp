namespace AbleSharp.GUI;

public static class ColorPalette
{
    private static readonly Dictionary<int, string> ColorMap = new()
    {
        { 0, "#FFCC00" }, // Gold 1
        { 1, "#FF3535" }, // Red
        { 2, "#FFA935" }, // Orange
        { 3, "#FFE135" }, // Yellow
        { 4, "#35FF35" }, // Green
        { 5, "#35FFA9" }, // Mint
        { 6, "#35E1FF" }, // Cyan
        { 7, "#3535FF" }, // Blue
        { 8, "#A935FF" }, // Purple
        { 9, "#FF35A9" }, // Pink
        { 10, "#FF3535" }, // Red (Alt)
        { 11, "#FFCC35" }, // Gold
        { 12, "#35FF81" }, // Sea Green
        { 13, "#35CCFF" }, // Light Blue
        { 14, "#8135FF" }, // Violet
        { 15, "#FF35D4" } // Magenta
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