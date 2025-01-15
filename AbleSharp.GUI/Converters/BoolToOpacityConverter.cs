﻿using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

public class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isTrue && isTrue ? 0.9 : 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}