using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LivePassword.Entities;
using LivePassword.Common;
using System.Collections.Generic;

namespace LivePassword.ViewModels
{
    // Gestisce la MainPage, l'Inizialize viene sempre chiamato in OnNavigatedTo e il DB viene gestito uguale
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Credentials = new ObservableCollection<KeyedList<string, CredentialSummary>>();
        }

        public void Initialize()
        {
            Credentials.Clear();

            var items = new List<CredentialSummary>();

            Sqlite.Database database;
            Sqlite.Statement statement;

            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM DataCredentialSummary", out statement);

            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                items.Add(new CredentialSummary
                {
                    Id = Sqlite.Sqlite3.sqlite3_column_int(statement, 0),
                    ItemId = Sqlite.Sqlite3.sqlite3_column_int(statement, 1),
                    Title = Sqlite.Sqlite3.sqlite3_column_text(statement, 2),
                    Subtitle = Sqlite.Sqlite3.sqlite3_column_text(statement, 3),
                    Table = Sqlite.Sqlite3.sqlite3_column_text(statement, 4),
                    Category = Sqlite.Sqlite3.sqlite3_column_text(statement, 5)
                   
                });
            }

            Sqlite.Sqlite3.sqlite3_finalize(statement);
            Sqlite.Sqlite3.sqlite3_close(database);
            TryDecrypt(items);
            var grouped =
                from credential in items
                orderby credential.Category
                //orderby credential.Title
                group credential by credential.Category
                    into groupedCredentials
                    select new KeyedList<string, CredentialSummary>(groupedCredentials);

            foreach (var key in grouped)
            {
                Credentials.Add(key);
            }
        }

        public void TryDecrypt(List<CredentialSummary> items)
        {
            if (items.Count(x => x.Category == "creditcard") > 0)
            {
                foreach (var item in items.Where(x => x.Category == "creditcard"))
                {
                    string value = item.Subtitle;
                    var salt = Crypto.CreateSalt();
                    byte[] data = Convert.FromBase64String(value);
                    string text = Crypto.DecryptAes(data, App.KEY, salt);
                    item.Subtitle = text;
                }
            } 
            if (items.Count(x => x.Category == "note") > 0)
            {
                foreach (var item in items.Where(x => x.Category == "note"))
                {
                    string value = item.Subtitle;
                    var salt = Crypto.CreateSalt();
                    byte[] data = Convert.FromBase64String(value);
                    string text = Crypto.DecryptAes(data, App.KEY, salt);
                    item.Subtitle = text;
                }
            }
        }
        public ObservableCollection<KeyedList<string, CredentialSummary>> Credentials { get; private set; }

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