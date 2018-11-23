using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LivePassword
{
    public class convImg : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string filename = value.ToString();
            filename = filename.Replace("ms-appdata:///Local/", "");
            BitmapImage bi = new BitmapImage();
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(filename))
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        bi.SetSource(fileStream);
                    }
            }
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
