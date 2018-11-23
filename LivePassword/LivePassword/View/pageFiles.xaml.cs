using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LivePassword.Resources;
using LivePassword.classes;
using Windows.Storage;
using Telerik.Windows.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using System.Collections.ObjectModel;

namespace LivePassword.View
{
    public partial class pageFiles : PhoneApplicationPage
    {
        public pageFiles()
        {
            InitializeComponent();
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareStorageItemsHandler);
        }
     
        private async void ShareStorageItemsHandler(DataTransferManager sender,
            DataRequestedEventArgs e)
        {
           if (_file != null)
           {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share File";
            request.Data.Properties.Description = "Share, "+_file.DisplayName;

            // Because we are making async calls in the DataRequested event handler,
            // we need to get the deferral first.
            DataRequestDeferral deferral = request.GetDeferral();

            // Make sure we always call Complete on the deferral.
            try
            {
                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(_file);
                request.Data.SetStorageItems(storageItems);
            }
            finally
            {
                deferral.Complete();
                _file = null;
            }

           }
        }

        ObservableCollection<FileList> newFile = new ObservableCollection<FileList>();
        classes.pFiles files = new classes.pFiles();
        public string _type = "";
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.PickSaveFileContinuation)
                {
                    StorageFile file = app.FilePickerContinuationArgs.Files.FirstOrDefault();
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
            }
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.PickFileContinuation)
                {
                    if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
                    {  
                        using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                        {
                            classes.pFiles _i = db.FileTable.FirstOrDefault(x => x.id == files.id);
                            classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == files.id);
                            
                        foreach (var _file in app.FilePickerContinuationArgs.Files)
                        {
                            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                            await _file.CopyAsync(localFolder, _file.Name, NameCollisionOption.ReplaceExisting);
                            BasicProperties bp = await _file.GetBasicPropertiesAsync();

                            _i.Files.Add(new classes.FileList(_file.Name, _i.id, bp.Size, _file.FileType, bp.DateModified.DateTime.ToLongDateString(), _file.FileType));
                            

                        }
                            _i.number = g.subtitle = _i.Files.Count + " " + _type;
                            db.SubmitChanges();
                            files = _i;
                        }
                        items.ItemsSource = null;
                        items.ItemsSource = files.Files;
                        items.Loaded += items_Loaded;
                        // Process file here

                        app.FilePickerContinuationArgs = null;
                    }
                }
            }
            RegisterForShare();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.delete;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.addimage;
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    
                    files = db.FileTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();

                    string t = NavigationContext.QueryString["type"].ToString();
                  _type = t;
                      NavigationContext.QueryString.Clear();                  
                    if (_type == "video")
                    {
                        txttype.Text = AppResources.video;
                    }
                    else if (_type == "audio")
                    {
                        txttype.Text = AppResources.audio;
                    }
                    else if (_type == "office")
                    {
                        txttype.Text = AppResources.office;
                    }
                    else if (_type.ToLower() == "files")
                    {
                        txttype.Text = AppResources.files;
                    }
                    LayoutRoot.DataContext = files;
                    items.ItemsSource = files.Files;
                }
            }
            base.OnNavigatedTo(e);
        }

        private void items_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {

            Clipboard.SetText(files.title);
            App.ToastMessage(AppResources.copied, AppResources.titlecopied);
        }

        private async void RadContextMenuItem_Tapped_1(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modfytitle;
            string message = AppResources.title;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pFiles _i = db.FileTable.FirstOrDefault(x => x.id == files.id);
                files.title = _i.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == files.id);
                g.title = y.Text;
                db.SubmitChanges();

                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = files;
            }
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            ctxmenu.IsOpen = true;
        }

        private async void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            //FileList f = (FileList)((Grid)(sender)).DataContext;
            //var ff = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            //foreach (var x in ff)
            //{
            //    string s = x.Name;
            //}
            //var file = await ApplicationData.Current.LocalFolder.GetFileAsync(f._name);
            //if (file != null)
            //{
            //    // Set the option to show the picker
            //    var options = new Windows.System.LauncherOptions();
            //    options.DisplayApplicationPicker = true;

            //    // Launch the retrieved file
            //    bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
            //    if (success)
            //    {
            //        // File launched
            //    }
            //    else
            //    {
            //        // File launch failed
            //    }
            //}
            //else
            //{
            //    // Could not find file
            //}
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pFiles).id).First();
               List<classes.FileList> ii = db.fileList.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pFiles).id).ToList();
                classes.pFiles _i = db.FileTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pFiles).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                db.fileList.DeleteAllOnSubmit(ii);
                db.FileTable.DeleteOnSubmit(_i);
                db.SubmitChanges();
                
            }
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            if (_type == "audio")
            {

                openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                openPicker.FileTypeFilter.Add(".wav");
                openPicker.FileTypeFilter.Add(".mp3");
            }
            else if (_type == "video")
            {
                openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                openPicker.FileTypeFilter.Add(".avi");
                openPicker.FileTypeFilter.Add(".mp4");
            }
            else if (_type == "image")
            {

                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".png");
            }
            else if (_type == "office")
            {

                openPicker.FileTypeFilter.Add(".doc");
                openPicker.FileTypeFilter.Add(".docx");
                openPicker.FileTypeFilter.Add(".xls");
                openPicker.FileTypeFilter.Add(".xlsx");
                openPicker.FileTypeFilter.Add(".ppt");
                openPicker.FileTypeFilter.Add(".pptx");
                openPicker.FileTypeFilter.Add(".one");
            }
            else
            {
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".doc");
                openPicker.FileTypeFilter.Add(".docx");
                openPicker.FileTypeFilter.Add(".xls");
                openPicker.FileTypeFilter.Add(".xlsx");
                openPicker.FileTypeFilter.Add(".ppt");
                openPicker.FileTypeFilter.Add(".pptx");
                openPicker.FileTypeFilter.Add(".one");
                openPicker.FileTypeFilter.Add(".avi");
                openPicker.FileTypeFilter.Add(".mp4");
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".rar");
                openPicker.FileTypeFilter.Add(".zip");
                openPicker.FileTypeFilter.Add(".pdf");
                openPicker.FileTypeFilter.Add(".jpeg");
            }
            openPicker.PickMultipleFilesAndContinue();
        }
     private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
        private async void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;
            var ff = await ApplicationData.Current.LocalFolder.GetFilesAsync();
           var file = await ApplicationData.Current.LocalFolder.GetFileAsync(f._name);
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
        StorageFile _file = null;
        private async void RadContextMenuItem_Tapped_3(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {

            FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;
           var file = await ApplicationData.Current.LocalFolder.GetFileAsync(f._name);
            _file = file;
            DataTransferManager.ShowShareUI();

        }

        private void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;
           
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == files.id);
                classes.FileList il = db.FileTable.FirstOrDefault(x => x.id == files.id).Files.FirstOrDefault(x => x.fid == f.fid);
                files.Files.Remove(files.Files.Where(x => x.fid == il.fid).FirstOrDefault());

                db.fileList.DeleteOnSubmit(il);
                db.SubmitChanges();
                classes.pFiles _i = db.FileTable.FirstOrDefault(x => x.id == files.id);
                _i = files;
                g.subtitle = files.number = _i.number = (string)(files.Files.Count).ToString() + " " + _type;
                db.SubmitChanges();
                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = files;

            }

            App.ToastMessage(AppResources.deleted, AppResources.filedeleted);
        }

   

        private async void RadContextMenuItem_Tapped_5(object sender, ContextMenuItemSelectedEventArgs e)
        {
            FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;
            var _file = await ApplicationData.Current.LocalFolder.GetFileAsync(f._name);

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedSaveFile = _file;
            savePicker.ContinuationData.Add("file", f._name);
            savePicker.FileTypeChoices.Add(_file.FileType+" file", new List<string>() {_file.FileType });
             savePicker.PickSaveFileAndContinue();
          
             if (_file != null)
             {
                 // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                 //CachedFileManager.DeferUpdates(_file);
                 //// write to file
                 //byte[] b = await ReadFile(_file);
                 //await FileIO.WriteBytesAsync(_file,b);

             }
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
            else
            {
           }
        } 

        public async Task<byte[]> ReadFile(StorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);

                    reader.ReadBytes(fileBytes);
                }

            }



            return fileBytes;
        }


    }
}