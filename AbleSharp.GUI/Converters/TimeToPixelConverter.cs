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
/// Gets zoom level from the DataContext's TimelineViewModel.
/// </summary>
public class TimeToPixelConverter : IValueConverter
{
    private static readonly ILogger _logger = LoggerService.GetLogger<TimeToPixelConverter>();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal dec)
        {
            // Get the zoom level from the TimelineViewModel
            var zoom = parameter is TimelineViewModel timeline ? timeline.Zoom : 80.0;
            _logger.LogDebug($"TimeToPixel: Value={dec}, Param Zoom={zoom} => result={(double)dec * zoom}");
            return (double)dec * zoom;
        }
        _logger.LogDebug($"TimeToPixel: value is not decimal. returning 0.0");
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}