using System;
using System.Diagnostics;
using System.Globalization;
using AbleSharp.GUI.Services;
using Avalonia.Data.Converters;
using AbleSharp.GUI.ViewModels;
using Microsoft.Extensions.Logging;

namespace AbleSharp.GUI.Converters;

/// <summary>
/// Converts a decimal "time in beats" to a pixel offset or width.
/// Gets zoom level from the TimelineViewModel parameter.
/// </summary>
public class TimeToPixelConverter : IValueConverter
{
    private static readonly ILogger _logger = LoggerService.GetLogger<TimeToPixelConverter>();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal time)
        {
            if (parameter is not TimelineViewModel timeline) throw new ArgumentException("TimeToPixel requires TimelineViewModel as a parameter");

            var zoom = timeline.Zoom;
            var pixels = (double)time * zoom;

            _logger.LogTrace($"TimeToPixel: Time={time}, Zoom={zoom} => Pixels={pixels}");
            return pixels;
        }

        throw new ArgumentException($"TimeToPixel: Invalid value type: {value?.GetType().Name ?? "null"}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}