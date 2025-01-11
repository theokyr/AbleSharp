using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AbleSharp.GUI.Converters
{
    public class TimeToPixelConverter : IValueConverter
    {
        public double PixelsPerBeat { get; set; } = 80; // Adjust as needed

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return (double)dec * PixelsPerBeat;
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}