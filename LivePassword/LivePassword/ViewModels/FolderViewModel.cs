using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using Newtonsoft.Json;
using LivePassword.Entities;
using System.Net;
using System.Collections.ObjectModel;
using LivePassword.Resources;
using LivePassword.Common;

namespace LivePassword.ViewModels
{
    public class FolderViewModel : INotifyPropertyChanged
    {
        public FolderViewModel()
        {
            Files = new ObservableCollection<FolderItem>();
            // Per il design-time
            if (DesignerProperties.IsInDesignTool)
            {
                Title = "My secret's files";
                Files.Add(new FolderItem 
                { 
                    DisplayName="MaCheCazzNeSo",
                    Size=23913211,
                    Extension=".doc",
                    ItemDate = DateTime.Now.Date.ToShortDateString()+", "+DateTime.Now.Date.ToShortTimeString()
                });
                Files.Add(new FolderItem
                {
                    DisplayName = "MaCheCazzNeSo",
                    Size = 23913211,
                    Extension = ".doc"
                }); 
                Files.Add(new FolderItem
                {
                    DisplayName = "MaCheCazzNeSo",
                    Size = 23913211,
                    Extension = ".doc"
                });
            }
        }

        public void InitializeAdd(string args)
        {
            var json = JsonConvert.DeserializeObject<Folder>(HttpUtility.UrlDecode(args));
            Id = json.Id;
            ItemId = json.IdCollection;
            SupportedFiles = json.SupportedFiles;
            Files = new ObservableCollection<FolderItem>();
        }
        public void InitializeGet(string args)
        {
            #region Default Initialize
            var json = JsonConvert.DeserializeObject<Folder>(HttpUtility.UrlDecode(args));
            Id = json.Id;
            ItemId = json.IdCollection;
            SupportedFiles = json.SupportedFiles;
            Title = json.Title;
            IdSummary = json.IdSummary;
            Files = new ObservableCollection<FolderItem>();
            Sqlite.Database database;
            Sqlite.Statement statement;
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
      
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("SELECT SupportedFiles FROM DataFolderType WHERE Id='{0}'",Id), out statement);
            if (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
                SupportedFiles = Sqlite.Sqlite3.sqlite3_column_text(statement, 0);
            // Finalizza la query e chiude il DB
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("SELECT * FROM FileItem WHERE CollectionId='{0}'", ItemId), out statement);
            while (Sqlite.Sqlite3.sqlite3_step(statement) == 100)
            {
                Files.Add(new FolderItem
                    {
                        Id = Sqlite.Sqlite3.sqlite3_column_int(statement, 0),
                        CollectionId = ItemId,
                        DisplayName = Sqlite.Sqlite3.sqlite3_column_text(statement, 2),
                        Size = (ulong)Sqlite.Sqlite3.sqlite3_column_int64(statement, 3),
                        Extension = Sqlite.Sqlite3.sqlite3_column_text(statement, 4),
                        LocalPath = Sqlite.Sqlite3.sqlite3_column_text(statement, 5),
                        ItemDate = Sqlite.Sqlite3.sqlite3_column_text(statement,6)
                    });
            }
            Sqlite.Sqlite3.sqlite3_finalize(statement);

            Sqlite.Sqlite3.sqlite3_close(database);
            #endregion

        }
        public void Save()
        {
            if (string.IsNullOrEmpty(Title))
            {
                IsValid = true; IsValid = false;
                return;
            }
            Sqlite.Database database;

            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

            var itemId = SaveItem(ref database);
            SaveFiles(ref database, itemId);
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
            

            string query = string.Format("INSERT INTO FileCollection (TypeId,Title) VALUES('{0}','{1}');", Id, Title);

            Sqlite.Sqlite3.sqlite3_prepare_v2(database, query, out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            long CollectionId = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);
            return CollectionId;
        }

        public void SaveFiles(ref Sqlite.Database database, long CollectionId)
        {
     
            foreach (var file in Files)
            {

                Sqlite.Statement statement;
                Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `FileItem` (CollectionId,DisplayName,Size,Extension,LocalPath,ItemDate) VALUES({0},'{1}',{2},'{3}','{4}','{5}');", CollectionId, file.DisplayName, file.Size, file.Extension, file.LocalPath, file.ItemDate), out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);
            }
        }
        // Salva il sommario per la mainpage, associandolo all'item precedentemente salvato
        private void SaveSummary(ref Sqlite.Database database, long itemId)
        {
            Sqlite.Statement statement;

            
            var subtitle = Files.Count.ToString() + " " + AppResources.files;

            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", itemId, Title, subtitle, "FileCollection", Id), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);
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
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        public int IdSummary { get; set; }
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

        public int ItemId { get; set; }

        public string SupportedFiles { get; set; }
        private bool isValid;
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
                NotifyPropertyChanged("IsValid");
            }
        }

        public ObservableCollection<FolderItem> Files { get; private set; }

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