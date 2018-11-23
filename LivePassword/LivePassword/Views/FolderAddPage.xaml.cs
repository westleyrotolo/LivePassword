using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage.Pickers;
using System.Collections.ObjectModel;
using LivePassword.Resources;
using Windows.Storage;
using Telerik.Windows.Controls;
using Windows.Storage.FileProperties;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using LivePassword.classes;
using System.Diagnostics;
using LivePassword.ViewModels;
using LivePassword.Entities;

namespace LivePassword.Views
{
    public partial class FolderAddPage : PhoneApplicationPage
    {
        public FolderViewModel ViewModel
        {
            get
            {
                return DataContext as FolderViewModel;
            }
        }

        public FolderAddPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ViewModel.InitializeAdd(NavigationContext.QueryString["args"]);

            }
            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
                {
                    foreach (var file in app.FilePickerContinuationArgs.Files)
                    {
                        StorageFile _file = await file.CopyAsync(localFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                        BasicProperties bp = await file.GetBasicPropertiesAsync();
                        ViewModel.Files.Add(new Entities.FolderItem(_file.DisplayName, bp.Size, file.FileType, _file.Name, bp.ItemDate.Date.ToShortDateString() + ", " + bp.ItemDate.TimeOfDay.Hours.ToString("00")+":"+bp.ItemDate.TimeOfDay.Minutes.ToString("00")));
                    }
                    app.FilePickerContinuationArgs = null;
               
                }
            }

        }




        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public string _type = "";
        //protected async override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    if (NavigationContext.QueryString.Count>0)
        //    {
        //        if (NavigationContext.QueryString.ContainsKey("id"))
        //        {
        //            string id = NavigationContext.QueryString["id"].ToString();
        //            _type = id;
        //            //if (_type == "video")
        //            //{
        //            //    txtType.Text = AppResources.pVideo;
        //            //}
        //            //else if (_type == "audio")
        //            //{
        //            //    txtType.Text = AppResources.pAudio;
        //            //}
        //            //else if (_type == "office")
        //            //{
        //            //    txtType.Text = AppResources.pOffice;
        //            //}
        //            //else if (_type == "files")
        //            //{
        //            //    txtType.Text = AppResources.pFiles;
        //            //}

        //        }
        //    }

        //    var app = App.Current as App;
        //    if (app.FilePickerContinuationArgs != null)
        //    {
        //        if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
        //        {
        //            foreach (var _file in app.FilePickerContinuationArgs.Files)
        //            {
        //                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        //                await _file.CopyAsync(localFolder, _file.Name, NameCollisionOption.ReplaceExisting);
        //                BasicProperties bp = await _file.GetBasicPropertiesAsync();

        //                file.Add(new classes.FileList( _file.Name, Guid.NewGuid().ToString(), bp.Size, _file.FileType, bp.DateModified.DateTime.ToLongDateString(), _file.FileType));
        //                items.Loaded += items_Loaded;
        //             }
        //            // Process file here

        //            app.FilePickerContinuationArgs = null;
        //        }
        //    }

        //    //((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.confirm;
        //    //((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.cancel;

        //    base.OnNavigatedTo(e);
        //}

        void items_Loaded(object sender, RoutedEventArgs e)
        {
            //items.BringIntoView(file.LastOrDefault());
      
            //items.Loaded -= items_Loaded;
        }

    //    ObservableCollection<classes.FileList> file = new ObservableCollection<classes.FileList>();

        //private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        //{
        //    if ((txttitle.Text == ""))
        //    {
        //        //if (txttitle.Text == "")
        //            //txttitle.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);

        //    }
        //    else
        //    {

        //        if ((_type == null) || (_type == ""))
        //            _type = "files";
        //        classes.pFiles files = new classes.pFiles();
        //        files.title = txttitle.Text;
        //        files.number = file.Count.ToString() + " " + _type;

        //        files.id = Guid.NewGuid().ToString();
        //        foreach (classes.FileList s in file)
        //        {
        //            files.Files.Add(new classes.FileList(s._name, files.id, s.dimension, s.thumbnail, s.lastmodifica, s.type));
        //        }
        //        classes.pGeneral g = new classes.pGeneral(files.id, files.title, files.number, _type.ToLower());
        //        using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
        //        {
                    
        //            db.FileTable.InsertOnSubmit(files);
        //            db.GeneralTable.InsertOnSubmit(g);
        //            db.SubmitChanges();
        //        }
        //        NavigationService.GoBack();
        //    }
        //}
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
        CoreApplicationView view = CoreApplication.GetCurrentView();

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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
            else if (_type=="video")
            {
                openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                openPicker.FileTypeFilter.Add(".avi");
                openPicker.FileTypeFilter.Add(".mp4");
            }
            else if (_type=="image")
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
        private async void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;
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

        //private void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        //{
        //    FileList f = (FileList)((RadContextMenuItem)(sender)).DataContext;

        //    file.Remove(file.Where(x => x.id == f.id).FirstOrDefault());
                
        //    App.ToastMessage(AppResources.deleted, AppResources.filedeleted);
        //}




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
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.GetFileAsync(f.LocalPath);
                await file.DeleteAsync();

            }
            catch (Exception)
            {

            }
 ViewModel.Files.Remove(f);
          
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    
    }
}
