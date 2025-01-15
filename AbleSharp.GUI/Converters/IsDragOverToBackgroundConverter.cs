using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AbleSharp.GUI.Converters;

public class IsDragOverToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isDragOver && isDragOver
            ? Brushes.LightBlue // Replace with your desired accent color
            : Brushes.LightGray; // Replace with your default background color
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}