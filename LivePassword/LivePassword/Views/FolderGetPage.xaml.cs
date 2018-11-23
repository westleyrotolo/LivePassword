using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LivePassword.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Foundation;
using Windows.ApplicationModel.Activation;
using LivePassword.Entities;
using LivePassword.Common;
using LivePassword.Resources;
using Windows.Storage.FileProperties;
using System.Collections.ObjectModel;
namespace LivePassword.Views
{
    public partial class FolderGetPage : PhoneApplicationPage
    {
        public FolderViewModel ViewModel
        {
            get
            {
                return DataContext as FolderViewModel;
            }
        }
        public FolderGetPage()
        {
            InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ViewModel.InitializeGet(NavigationContext.QueryString["args"]);
            } var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
                {
                    Sqlite.Database database;
                    Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
                    Sqlite.Statement statement;
                    foreach (var file in app.FilePickerContinuationArgs.Files)
                    {
                        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                        
                        StorageFile _file =  await file.CopyAsync(localFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                        BasicProperties bp = await file.GetBasicPropertiesAsync();

                        Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `FileItem` (CollectionId,DisplayName,Size,Extension,LocalPath,ItemDate) VALUES({0},'{1}','{2}','{3}','{4}','{5}');", ViewModel.ItemId, file.DisplayName, bp.Size, file.FileType,_file.Name, bp.ItemDate.Date.ToShortDateString() + ", " + bp.ItemDate.TimeOfDay.Hours.ToString("00") + ":" + bp.ItemDate.TimeOfDay.Minutes.ToString("00")), out statement);
                        Sqlite.Sqlite3.sqlite3_step(statement);
                        Sqlite.Sqlite3.sqlite3_finalize(statement);
        
                        ViewModel.Files.Add(new Entities.FolderItem(_file.DisplayName, bp.Size, file.FileType, _file.Name,bp.ItemDate.Date.ToShortDateString()+", "+bp.ItemDate.Date.ToShortTimeString()));
                    }
                    string subtitile = ViewModel.Files.Count + " " + AppResources.files;
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE DataCredentialSummary SET Subtitle = '{0}' WHERE Id = {1}", subtitile, ViewModel.IdSummary), out statement);
                    App.MergeAvailable = true;
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    Sqlite.Sqlite3.sqlite3_close(database);
                    app.FilePickerContinuationArgs = null;

                }
            }

            RegisterForShare();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareStorageItemsHandler);
        }
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var extensions = ViewModel.SupportedFiles.Split(',');

            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            foreach (var extension in extensions)
            {
                picker.FileTypeFilter.Add(extension);
            }

            picker.PickMultipleFilesAndContinue();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            Entities.FolderItem f = (Entities.FolderItem)((Microsoft.Phone.Controls.MenuItem)(sender)).DataContext;
            ViewModel.Files.Remove(f);
            try
            {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file =await storageFolder.GetFileAsync(f.LocalPath);
            await file.DeleteAsync();
       
            }
            catch (Exception)
            {
                
            }
             Sqlite.Database database;
            Sqlite.Statement statement;
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("DELETE FROM FileItem WHERE Id={0}", f.Id), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            // Finalizza la query e chiude il DB
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            string subtitile = ViewModel.Files.Count + " " + AppResources.files;
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE DataCredentialSummary SET Subtitle = '{0}' WHERE Id = {1}",subtitile, ViewModel.IdSummary), out statement);
            Sqlite.Sqlite3.sqlite3_step(statement);
            Sqlite.Sqlite3.sqlite3_finalize(statement);
            Sqlite.Sqlite3.sqlite3_close(database);
            App.MergeAvailable = true;

        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            FolderItem f = (FolderItem)((MenuItem)(sender)).DataContext;
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(f.LocalPath);
            if (file != null)
            {
                // Set the option to show the picker
                var options = new Windows.System.LauncherOptions();
                options.DisplayApplicationPicker = true;

                // Launch the retrieved file
                bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
                if (success)
                {
                    // File launched
                }
                else
                {
                    // File launch failed
                }
            }
            else
            {
                // Could not find file
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            Entities.FolderItem f = (Entities.FolderItem)((Microsoft.Phone.Controls.MenuItem)(sender)).DataContext;
            var _file = await ApplicationData.Current.LocalFolder.GetFileAsync(f.LocalPath);
            
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedSaveFile = _file;
            savePicker.ContinuationData.Add("file", f.LocalPath);
            savePicker.FileTypeChoices.Add(_file.FileType + " file", new List<string>() { _file.FileType });
            savePicker.PickSaveFileAndContinue();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var item = button.DataContext as FolderViewModel;

            if (item != null)
            {
                Clipboard.SetText(item.Title);
                App.ToastMessage(string.Empty, AppResources.copied);
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var item = button.DataContext as FolderViewModel;

            if (item != null)
            {
#warning Bisogna aggiungere le localizzazioni

                var result = await DialogService.ShowInputAsync("Live Password", "Enter new value for " + AppResources.ResourceManager.GetString(item.Title), item.Title);
                if (result != null && result != item.Title)
                {
                    item.Title = result;

                    Sqlite.Database database;
                    Sqlite.Statement statement;

                    Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

                        Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE DataCredentialSummary SET Title = '{0}    ' WHERE Id = {1}",item.Title, ViewModel.IdSummary), out statement);
                        Sqlite.Sqlite3.sqlite3_step(statement);
                        Sqlite.Sqlite3.sqlite3_finalize(statement);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE FileCollection SET Title = '{0}' WHERE Id = {1}",  item.Title.Replace("'", "''"), ViewModel.IdSummary), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    Sqlite.Sqlite3.sqlite3_close(database);
                    App.MergeAvailable = true;
                }
            }
        }
        StorageFile TempFile = null;
        private async void Share_Click(object sender, RoutedEventArgs e)
        {
            FolderItem f = (FolderItem)((MenuItem)(sender)).DataContext;
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(f.LocalPath);
            TempFile = file;
            DataTransferManager.ShowShareUI();
        }
        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            StorageFile file = args.File;
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync. 
                CachedFileManager.DeferUpdates(file);
                // write to file 
                await FileIO.WriteTextAsync(file, file.Name);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file. 
                // Completing updates may require Windows to ask for user input. 
            }
        }
        private void ShareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (TempFile != null)
            {
                DataRequest request = e.Request;
                request.Data.Properties.Title = "Share File";
                request.Data.Properties.Description = "Share, " + TempFile.DisplayName;

                // Because we are making async calls in the DataRequested event handler,
                // we need to get the deferral first.
                DataRequestDeferral deferral = request.GetDeferral();

                // Make sure we always call Complete on the deferral.
                try
                {
                    List<IStorageItem> storageItems = new List<IStorageItem>();
                    storageItems.Add(TempFile);
                    request.Data.SetStorageItems(storageItems);
                }
                finally
                {
                    deferral.Complete();
                    TempFile = null;
                }

            }
        }

        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<FolderItem> files = new ObservableCollection<FolderItem>(ViewModel.Files);
                foreach (var f in files)
                {
                    try
                    {
                        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                        StorageFile file = await storageFolder.GetFileAsync(f.LocalPath);
                        await file.DeleteAsync();
                        ViewModel.Files.Remove(f);

                    }
                    catch (Exception)
                    {

                    }
                }
                Sqlite.Database database;
                Sqlite.Statement statement;
                Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

                Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("DELETE FROM DataCredentialSummary WHERE Id="+ViewModel.IdSummary), out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);

                Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("DELETE FROM FileItem WHERE CollectionId=", ViewModel.ItemId), out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);

                Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("DELETE FROM FileCollection WHERE Id=", ViewModel.ItemId), out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);

                Sqlite.Sqlite3.sqlite3_close(database);
                App.MergeAvailable = true;
                NavigationService.GoBack();
            }
        }
    }
