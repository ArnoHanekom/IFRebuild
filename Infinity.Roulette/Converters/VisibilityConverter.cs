using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Infinity.Roulette.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)((bool)value ? 0 : 2);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible;
    }
}
