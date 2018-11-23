using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace LivePassword.Common
{
    // Estensioni per la navigatione, navighi ad una pagina passandole un oggetto, che viene serializzato
    // in JSON
    public static class Extensions
    {
        public static bool Navigate(this NavigationService frame, string page, object args)
        {
            var json = JsonConvert.SerializeObject(args);
            return frame.Navigate(new Uri(page + "?args=" + HttpUtility.UrlEncode(json), UriKind.Relative));
        }

        public static bool Navigate(this PhoneApplicationFrame frame, string page, object args)
        {
            var json = JsonConvert.SerializeObject(args);
            return frame.Navigate(new Uri(page + "?args=" + HttpUtility.UrlEncode(json), UriKind.Relative));
        }

        public static T ValueOrDefault<T>(this JToken token, T defaultValue)
        {
            var value = token.Value<T>();
            if (value != null)
            {
                return value;
            }

            return defaultValue;
        }
    }
}
