using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword
{
    public static class DataConverter
    {
        public const long KILOBYTE = 1024;
        public const long MEGABYTE = 1048576;
        public const long GIGABYTE = 1073741824;
        public const long TERABYTE = 1099511628000;
        public static string Convert(long value)
        {

            var lenght = value;

            if (lenght >= KILOBYTE && lenght < MEGABYTE)
                return lenght / KILOBYTE + " KB";

            if (lenght >= MEGABYTE && lenght < GIGABYTE)
                return (lenght / KILOBYTE) / KILOBYTE + " MB";

            if (lenght >= GIGABYTE && lenght < TERABYTE)
                return ((lenght / KILOBYTE) / KILOBYTE) / KILOBYTE + " GB";

            if (lenght >= TERABYTE)
                return (((lenght / KILOBYTE) / KILOBYTE) / KILOBYTE) / KILOBYTE + " TB";

            return lenght + " B";
        }
    }
}
