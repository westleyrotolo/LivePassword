using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LivePassword.Converters
{
    // Converte stringhe in formato MM/yy a DateTime e viceversa
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            return DateTime.ParseExact((string)value, "MM/yy", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).ToString("MM/yy");
        }
    }
}