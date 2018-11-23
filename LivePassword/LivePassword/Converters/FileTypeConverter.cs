using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LivePassword.Converters
{
    public class FileTypeConverter : IValueConverter
    {
        const string PATH = "/Assets/Files/";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return PATH+"file.png";

            switch (value.ToString().ToLower())
            {
                case ".mp3":
                    return PATH + "audio.png";
                case ".avi": 
                case ".mp4": 
                    return PATH + "video.png";
                case ".xls": 
                case ".xlsx":
                    return PATH + "excel.png";
                case ".doc": 
                case ".docx":
                    return PATH + "word.png";
                case ".ppt": 
                case ".pptx":
                    return PATH + "powerpoint.png";
                case ".one":
                    return PATH + "onenote.png";
                case ".pdf": 
                    return PATH + "pdf.png";
                case ".rar":
                    return PATH + "rar.png";
                case ".txt":
                    return PATH + "text.png";
                case ".zip":
                    return PATH + "zip.png";
                case ".png": 
                case ".jpg": 
                case ".jpeg":
                    return PATH + "image.png";
                default:
                    return PATH + "/file.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
