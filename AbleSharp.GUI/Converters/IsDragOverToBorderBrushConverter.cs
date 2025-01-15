﻿using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AbleSharp.GUI.Converters;

public class IsDragOverToBorderBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isDragOver && isDragOver
            ? Brushes.LightGray
            : Brushes.Gray; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}