using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AbleSharp.GUI.Converters;

public class IsDragOverToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isDragOver && isDragOver
            ? Brushes.DarkSlateGray
            : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}