using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

/// <summary>
/// Converts indent level to a margin value for track hierarchy display
/// </summary>
public class IndentConverter : IValueConverter
{
    private const double IndentSize = 20.0; // pixels per indent level

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal level) return new Avalonia.Thickness((double)level * IndentSize, 0, 0, 0);

        return new Avalonia.Thickness(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}