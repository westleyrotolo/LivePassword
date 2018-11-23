using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LivePassword.Entities;
using LivePassword.Common;

namespace LivePassword.ViewModels
{
    // Catalogo dei tipi di credenziali (E poi di cartelle di file) creabili
    public class CatalogViewModel : INotifyPropertyChanged
    {
        public CatalogViewModel()
        {
            Credentials = new KeyedList<string, ICatalogItem>("Credentials");
            Folders = new KeyedList<string, ICatalogItem>("files");

            Groups = new ObservableCollection<KeyedList<string, ICatalogItem>>()
            {
                Credentials,
                Folders
            };

            // Per il design-time
            if (DesignerProperties.IsInDesignTool)
            {
                Credentials.Add(new Credential { Id = "creditcard", Table = "CredentialCreditCard" });
            }
        }

        // Inizializza il viewmodel, viene usato nell'OnNavigatedTo della pagina
        public void Initialize()
        {
            // Pulisce per evitare doppioni
            Credentials.Clear();
            Folders.Clear();

            Sqlite.Database database;
            Sqlite.Statement statement;

            // Apre il DB e prepara la query (2 | 4 == Read | Write)
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

            #region CretentialType
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM DataCredentialType", out statement);

            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                Credentials.Add(new Credential
                {
                    Id = Sqlite.Sqlite3.sqlite3_column_text(statement, 0),
                    Table = Sqlite.Sqlite3.sqlite3_column_text(statement, 1),
                    SummaryKey = Sqlite.Sqlite3.sqlite3_column_text(statement, 2)
                });
            }

            Sqlite.Sqlite3.sqlite3_finalize(statement);
            #endregion

            #region Folder
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT * FROM DataFolderType", out statement);
            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                Folders.Add(new Folder
                {
                    Id = Sqlite.Sqlite3.sqlite3_column_text(statement, 0),
                    SupportedFiles = Sqlite.Sqlite3.sqlite3_column_text(statement, 1),
                });
            }

            // Finalizza la query e chiude il DB
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            #endregion

            Sqlite.Sqlite3.sqlite3_close(database);
        }

        public KeyedList<string, ICatalogItem> Credentials { get; private set; }

        public KeyedList<string, ICatalogItem> Folders { get; private set; }

        public ObservableCollection<KeyedList<string, ICatalogItem>> Groups { get; private set; }

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