using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AbleSharp.GUI.ViewModels;

namespace AbleSharp.GUI.Converters;

/// <summary>
/// Converts a decimal "time in beats" to a pixel offset or width.
/// Gets zoom level from the DataContext's TimelineViewModel.
/// </summary>
public class TimeToPixelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal dec)
        {
            // Get the zoom level from the TimelineViewModel
            var zoom = parameter is TimelineViewModel timeline ? timeline.Zoom : 80.0;
            return (double)dec * zoom;
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}