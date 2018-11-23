using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.ViewModels
{
    public class UserSource
    {
        public UserSource()
        {
           
        }
        public void InitalizeAdd()
        {
            Create();
        }   
        private ObservableCollection<string> _users = new ObservableCollection<string>();
        public ObservableCollection<string> Users
        {
            get
            {
                return _users;
            }
        }

        public void Create()
        {
            Sqlite.Database database;
            Sqlite.Statement statement2;
            // Carica tutti i field disponibili per questo tipo di credenziale (Magari la query sarebbe da sostituire con uno string.format
            // E ancora meglio si potrebbe fare una classe con tutte le query CONST
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

            _users.Clear();
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT User FROM CredentialLogin UNION Select Mail AS User From CredentialMail UNION Select User From CredentialWebSite", out statement2);
            while (Sqlite.Sqlite3.sqlite3_step(statement2) == 100)
            {
                if (!(_users.Contains(Sqlite.Sqlite3.sqlite3_column_text(statement2, 0))))
                    _users.Add(Sqlite.Sqlite3.sqlite3_column_text(statement2, 0));
            }
            Sqlite.Sqlite3.sqlite3_finalize(statement2);
            Sqlite.Sqlite3.sqlite3_close(database);
        }
    }
}
