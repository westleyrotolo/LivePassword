using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LivePassword.Resources;
namespace LivePassword
{
    public class TitleConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().ToLower() == "images")
                return AppResources.images;
            if (value.ToString().ToLower() == "website")
                return AppResources.website;
            if (value.ToString().ToLower() == "note")
                return AppResources.note;
            if (value.ToString().ToLower() == "mail")
                return AppResources.mail;
            if (value.ToString().ToLower() == "login")
                return AppResources.login;
            if (value.ToString().ToLower() == "files")
                return AppResources.files;
            if (value.ToString().ToLower() == "audio")
                return AppResources.audio;
            if (value.ToString().ToLower() == "video")
                return AppResources.video;
            if (value.ToString().ToLower() == "office")
                return AppResources.office;
                
                return AppResources.creditcard;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
