using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AbleSharp.GUI.Converters;

public class IsDragOverToBorderBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isDragOver && isDragOver
            ? Brushes.Blue // Replace with your desired accent border color
            : Brushes.Gray; // Replace with your default border color
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}