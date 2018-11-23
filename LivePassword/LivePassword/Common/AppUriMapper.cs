using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace LivePassword.Common
{
    public class AppUriMapper : UriMapperBase
    {
        // Prima di tutto:
        // Qua viene gestita la navigazione,
        // Se l'app viene aperta da zero, o dopo un tombstone, viene richiesta la password.
        // Se l'app viene fatta passare in background e si riapre?
        // Dipende se è andata in tombstone o no, poi metto qualcosa per recuperare la pagina precedente
        // Dopo il login
        public override Uri MapUri(Uri uri)
        {
            if (!App.IsLoggedIn)
            {
                return new Uri("/Views/LoginPage.xaml?redirect=" + HttpUtility.UrlEncode(uri.ToString()), UriKind.Relative);
            }

            return uri;
        }
    }
}
