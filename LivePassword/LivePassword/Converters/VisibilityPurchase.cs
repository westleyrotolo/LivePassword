using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LivePassword.Converters
{
    public class VisibilityPurchase : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value.ToString().ToLower().Contains("cred"))
                return Visibility.Collapsed;
            bool productStatus = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses["LivePasswordPro"].IsActive;
            if (productStatus)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
