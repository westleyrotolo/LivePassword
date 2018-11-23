using LivePassword.Resources;
using System;
using System.Linq;
using System.Windows.Data;

namespace LivePassword.Converters
{
    // Resource key to localized string (Per mantenere il DB localizzato)
    public class ResourceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (string)value;
            if (str == null)
                return null;

            var param = (string)parameter;

            if (param != null)
            {
                return param + AppResources.ResourceManager.GetString(str);
            }

            return AppResources.ResourceManager.GetString(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}