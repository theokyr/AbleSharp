using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;
using AbleSharp.GUI.Services;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.ViewModels;
using Avalonia;
using Avalonia.Data;

namespace AbleSharp.GUI.Converters;

public class TimeToPixelMultiConverter : IMultiValueConverter
{
    private static readonly ILogger _logger = LoggerService.GetLogger<TimeToPixelMultiConverter>();

    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2)
        {
            _logger.LogError("TimeToPixelMultiConverter: Insufficient binding values provided.");
            return BindingOperations.DoNothing;
        }

        if (values[0] is not decimal time)
        {
            _logger.LogError($"TimeToPixelMultiConverter: First value must be of type decimal, but was {values[0]?.GetType().Name ?? "null"}.");
            return BindingOperations.DoNothing;
        }

        if (values[1] is not double zoom)
        {
            _logger.LogError($"TimeToPixelMultiConverter: Second value must be of type double, but was {values[1]?.GetType().Name ?? "null"}.");
            return BindingOperations.DoNothing;
        }

        // Perform the conversion
        var pixels = (double)time * zoom;

        _logger.LogTrace($"TimeToPixelMultiConverter: Time={time}, Zoom={zoom} => Pixels={pixels}");
        return pixels;
    }

    public IList<object?> ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("TimeToPixelMultiConverter does not support ConvertBack.");
    }
}