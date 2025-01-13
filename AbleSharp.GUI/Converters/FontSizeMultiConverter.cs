using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

public class FontSizeMultiConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double zoom && parameter is string baseSizeStr)
            if (double.TryParse(baseSizeStr, out var baseSize))
                return Math.Max(8.0, baseSize * zoom); // Minimum font size of 8

        return 10.0; // Default font size
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}