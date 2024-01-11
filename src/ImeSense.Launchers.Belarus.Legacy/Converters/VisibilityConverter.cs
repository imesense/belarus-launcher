using System.Globalization;
using System.Windows.Data;

namespace ImeSense.Launchers.Belarus.Converters;

public class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var isVisible = (bool) value;
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        var visibility = (Visibility) value;
        return (visibility == Visibility.Visible);
    }
}
