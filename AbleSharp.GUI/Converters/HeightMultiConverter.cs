using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

public class HeightMultiConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double zoom && parameter is string baseHeightStr)
            if (double.TryParse(baseHeightStr, out var baseHeight))
                return baseHeight * zoom;

        return 40.0; // Default height
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}