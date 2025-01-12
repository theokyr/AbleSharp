using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters;

/// <summary>
/// Converts a decimal "time in beats" to a pixel offset or width.
/// For example: Start or End is decimal, multiplied by PixelsPerBeat.
/// </summary>
public class TimeToPixelConverter : IValueConverter
{
    public double PixelsPerBeat { get; set; } = 80.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal dec)
        {
            return (double)dec * PixelsPerBeat;
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}