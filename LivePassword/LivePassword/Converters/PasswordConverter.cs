using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LivePassword.Common;
namespace LivePassword.Converters
{
    public class PasswordConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {

                return new string(GetPassword((string)value).ToArray());
            }

            return GetPassword("yolo hihi");
        }

        private IEnumerable<char> GetPassword(string original)
        {
            foreach (var ch in original)
            {
                yield return '•';
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
