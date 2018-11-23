using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LivePassword.Common;
using Newtonsoft.Json;
using LivePassword.Entities;
using System.Net;
using System.Collections.Generic;
using LivePassword.Resources;
using System.Windows;

namespace LivePassword.ViewModels
{
    // Ora viene il bello. Questo ViewModel gestisce SIA l'aggiunta SIA la lettura
    public class CredentialViewModel : INotifyPropertyChanged
    {
        public CredentialViewModel()
        {
            Fields = new ObservableCollection<FieldViewModel>();
            users = new List<string>();
             if (DesignerProperties.IsInDesignTool)
            {
                Fields.Add(new FieldViewModel { DisplayName = "title", Value = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", InputScope = InputScopeNameValue.Chat });
                Fields.Add(new FieldViewModel { DisplayName = "title", Value = "LOUJTTT ipsum dolor sit amet.", InputScope = InputScopeNameValue.Password });
                Fields.Add(new FieldViewModel { DisplayName = "title", Value = "arrowgreen@live.it", InputScope = InputScopeNameValue.EmailSmtpAddress });
                Fields.Add(new FieldViewModel { DisplayName = "title", IsValid=false,  InputScope = InputScopeNameValue.Date });
            }
        }

        // Inizializza per l'aggiunta. L'args è l'oggetto che viene passato con l'extension
        public void InitializeAdd(string args)
        {
            var json = JsonConvert.DeserializeObject<Credential>(HttpUtility.UrlDecode(args));
            Id = json.Id;
            ItemId = json.ItemId;
            Table = json.Table;
            SummaryKey = json.SummaryKey;
            Sqlite.Database database;
            Sqlite.Statement statement;
            // Carica tutti i field disponibili per questo tipo di credenziale (Magari la query sarebbe da sostituire con uno string.format
            // E ancora meglio si potrebbe fare una classe con tutte le query CONST
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM DataCredentialField WHERE CredentialId = \"" + Id + "\"", out statement);
            
            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                Fields.Add(new FieldViewModel
                {
                    ColumnName = Sqlite.Sqlite3.sqlite3_column_text(statement, 2),
                    ColumnIndex = Sqlite.Sqlite3.sqlite3_column_int(statement, 3),
                    DisplayName = Sqlite.Sqlite3.sqlite3_column_text(statement, 4),
                    Placeholder = Sqlite.Sqlite3.sqlite3_column_text(statement, 5),
                    InputScope = (InputScopeNameValue)Sqlite.Sqlite3.sqlite3_column_int(statement, 6), // L'inputscope viene salvato come INT
                });
            }

            Sqlite.Sqlite3.sqlite3_finalize(statement);
            Sqlite.Sqlite3.sqlite3_close(database);
        }

        // qua inizializza la lettura
        public void InitializeGet(string args)
        {
            // questa parte è uguale all'altra (Ma l'ho messa direttamente per non inizializzare 2 volte il DB)
            #region Default Initialize
            var json = JsonConvert.DeserializeObject<Credential>(HttpUtility.UrlDecode(args));
            Id = json.Id;
            ItemId = json.ItemId;
            Table = json.Table;
            IdSummary = json.IdSummary;
           
            if (Id == "Identity")
                Id = Table.Replace("Credential", "").ToString().ToLower();
            Sqlite.Database database;
            Sqlite.Statement statement;

            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM DataCredentialField WHERE CredentialId = \"" + Id + "\"", out statement);

            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                Fields.Add(new FieldViewModel
                {
                    ColumnName = Sqlite.Sqlite3.sqlite3_column_text(statement, 2),
                    ColumnIndex = Sqlite.Sqlite3.sqlite3_column_int(statement, 3),
                    DisplayName = Sqlite.Sqlite3.sqlite3_column_text(statement, 4),
                    Placeholder = Sqlite.Sqlite3.sqlite3_column_text(statement, 5),
                    InputScope = (InputScopeNameValue)Sqlite.Sqlite3.sqlite3_column_int(statement, 6),
                });
            }
            Sqlite.Sqlite3.sqlite3_finalize(statement); 
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("SELECT SummaryKey FROM DataCredentialType WHERE Id = '{0}'",Id), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
                  SummaryKey = Sqlite.Sqlite3.sqlite3_column_text(statement, 0);
                  Sqlite.Sqlite3.sqlite3_finalize(statement);
            #endregion

            // Qua va a prendere l'elemento da visualizzare dalla sua tabella
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM " + Table + " WHERE Id = " + ItemId, out statement);

            // e qua, se lo trova, loopa i field per prendere i vari valori
            if (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                foreach (var field in Fields)
                {
                    field.Value = Sqlite.Sqlite3.sqlite3_column_text(statement, field.ColumnIndex);
                
                }
            }
            TryDecrypt();
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            Sqlite.Sqlite3.sqlite3_close(database);
        }
        public void TryEncrypt()
        {
            
            var salt = Crypto.CreateSalt();
            if (Fields.Count(x => x.InputScope == InputScopeNameValue.Password) > 0)
            {
               Fields.Where(x => x.InputScope == InputScopeNameValue.Password).First().Value = Convert.ToBase64String(Crypto.EncryptAes(Fields.Where(x => x.InputScope == InputScopeNameValue.Password).First().Value, App.KEY, salt));
            } 
            if (Fields.Count(x => x.DisplayName == "note") > 0)
            {
                Fields.Where(x => x.DisplayName == "note").First().Value = Convert.ToBase64String(Crypto.EncryptAes(Fields.Where(x => x.DisplayName == "note").First().Value, App.KEY, salt));
            }
            if (Fields.Count(x => x.DisplayName == "number") > 0)
            {
                Fields.Where(x => x.DisplayName == "number").First().Value = Convert.ToBase64String(Crypto.EncryptAes(Fields.Where(x => x.DisplayName == "number").First().Value, App.KEY, salt));
            }
            if (Fields.Count(x => x.DisplayName == "cvv") > 0)
            {
                Fields.Where(x => x.DisplayName == "cvv").First().Value = Convert.ToBase64String(Crypto.EncryptAes(Fields.Where(x => x.DisplayName == "cvv").First().Value, App.KEY, salt));
            }
            if (Fields.Count(x => x.DisplayName == "pin") > 0)
            {
                Fields.Where(x => x.DisplayName == "pin").First().Value = Convert.ToBase64String(Crypto.EncryptAes(Fields.Where(x => x.DisplayName == "pin").First().Value, App.KEY, salt));
            }


        }
        public void TryDecrypt()
        {
            if (Fields.Count(x => x.InputScope == InputScopeNameValue.Password) > 0)
            {
                string value = Fields.Where(x => x.InputScope == InputScopeNameValue.Password).First().Value;
                var salt = Crypto.CreateSalt();
                byte[] data = Convert.FromBase64String(value);
                string password = Crypto.DecryptAes(data, App.KEY, salt);
                Fields.Where(x => x.InputScope == InputScopeNameValue.Password).First().Value = password;
            }
            if (Fields.Count(x => x.DisplayName == "note") > 0)
            {
                string value = Fields.Where(x => x.DisplayName == "note").First().Value;
                var salt = Crypto.CreateSalt();
                byte[] data = Convert.FromBase64String(value);
                string text = Crypto.DecryptAes(data, App.KEY, salt);
                Fields.Where(x => x.DisplayName == "note").First().Value = text;
            }
            if (Fields.Count(x => x.DisplayName == "number") > 0)
            {
                string value = Fields.Where(x => x.DisplayName == "number").First().Value;
                var salt = Crypto.CreateSalt();
                byte[] data = Convert.FromBase64String(value);
                string text = Crypto.DecryptAes(data, App.KEY, salt);
                Fields.Where(x => x.DisplayName == "number").First().Value = text;
            }
            if (Fields.Count(x => x.DisplayName == "cvv") > 0)
            {
                string value = Fields.Where(x => x.DisplayName == "cvv").First().Value;
                var salt = Crypto.CreateSalt();
                byte[] data = Convert.FromBase64String(value);
                string text = Crypto.DecryptAes(data, App.KEY, salt);
                Fields.Where(x => x.DisplayName == "cvv").First().Value = text;
            }
            if (Fields.Count(x => x.DisplayName == "pin") > 0)
            {
                string value = Fields.Where(x => x.DisplayName == "pin").First().Value;
                var salt = Crypto.CreateSalt();
                byte[] data = Convert.FromBase64String(value);
                string text = Crypto.DecryptAes(data, App.KEY, salt);
                Fields.Where(x => x.DisplayName == "pin").First().Value = text;
            }
        }
        // Salvataggio del NUOVO elemento (Forse sarebbe meglio spostarlo nella pagina, per uniformità, ma vbb
        public void Save()
        {

            if (Fields.Any(x => x.IsValid != true))
            {
                foreach (var field in Fields)
                    if (string.IsNullOrEmpty(field.Value))
                    {
                        field.IsValid = true;
                        field.IsValid = false;
                    }
                return;
            }

            TryEncrypt();
            Sqlite.Database database;
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

            var itemId = SaveItem(ref database);
            if ((Table == "CredentialDriverLicense") || (Table == "CredentialIdentityCard") || (Table == "CredentialPassport") || (Table == "CredentialSocialSecurityNumber"))
                SaveSummaryIdentity(ref database, itemId);
            else
                SaveSummary(ref database, itemId);

            Sqlite.Sqlite3.sqlite3_close(database);
        
            App.MergeAvailable = true;
            App.RootFrame.RemoveBackEntry();
            App.RootFrame.GoBack();
        }

        // Salva l'elemento e ritorna il suo ID
        private long SaveItem(ref Sqlite.Database database)
        {
            Sqlite.Statement statement;

            var queryKeys = new string[Fields.Count+2];
            var queryValues = new string[Fields.Count+2];

            // Crea la query. Attenzione: nelle query dirette gli apici singoli vanno raddoppiati per fare l'escape
            for (int i = 0; i < Fields.Count; i++)
            {
                queryKeys[i] = Fields[i].ColumnName;
                queryValues[i] = "'" + Fields[i].Value.Replace("'", "''") + "'";
            }
            queryKeys[Fields.Count] = "Version";
            queryValues[Fields.Count] ="'"+ DateTime.Now.Ticks.ToString()+"'";
            queryKeys[Fields.Count + 1] = "IdSync";
            queryValues[Fields.Count + 1] = "'"+Guid.NewGuid().ToString()+"'";
            string query = string.Format("INSERT INTO `{0}` ({1}) VALUES({2});", Table, string.Join(",", queryKeys), string.Join(",", queryValues));
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, query, out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);

            return Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);
        }
        public string TableToString(string Table)
        {
            if (Table == "CredentialDriverLicense")
                return "DriverLicense";
            if (Table == "CredentialIdentityCard")
                return "IdentityCard";
            if (Table == "CredentialPassport")
                return "Passport";
            return "SocialSecurityNumber";
        }
        public void SaveSummaryIdentity(ref Sqlite.Database database, long itemId)
        {
            Sqlite.Statement statement;
            var subtitle = TableToString(Table).ToLower();
            subtitle = AppResources.ResourceManager.GetString(subtitle);
            subtitle = subtitle.Replace("'", "''");
            var title = Fields.FirstOrDefault(x => x.ColumnName == "Name").Value.Replace("'", "''");

            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','Identity');", itemId, title, subtitle, Table), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);
        }
        // Salva il sommario per la mainpage, associandolo all'item precedentemente salvato
        private void SaveSummary(ref Sqlite.Database database, long itemId)
        {
            Sqlite.Statement statement;

            var title = Fields.FirstOrDefault(x => x.ColumnName == "Title").Value.Replace("'", "''");

            var subtitle = Fields.FirstOrDefault(x => x.ColumnName == SummaryKey).Value;

            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", itemId, title, subtitle, Table, Id), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);
        }

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        public int IdSummary { get; set; }

        public int ItemId { get; set; }

        public string Thumbnail { get; set; }

        public string Table { get; set; }

        public string SummaryKey { get; set; }
        private Visibility error = Visibility.Collapsed;
        public Visibility Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                NotifyPropertyChanged("Error");
            }
        }
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new RelayCommand(Save);

                return _saveCommand;
            }
        }
        public List<string> users { get;  set; }
        public ObservableCollection<FieldViewModel> Fields { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}