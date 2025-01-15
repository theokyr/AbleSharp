using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

public class GreaterThanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && int.TryParse(parameter as string, out var compareTo)) return intValue > compareTo;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}