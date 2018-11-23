using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LivePassword.Converters
{
    // Serve per l'empty content della LLS, se non ci sono elementi viene mostrato il TextBlock, se no no
    public class ItemsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}