using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Telerik.Windows.Controls;
using LivePassword.Resources;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace LivePassword.View
{
    public partial class pagePhoto : PhoneApplicationPage
    {

        PhotoChooserTask photoChooserTask;
        public pagePhoto()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
           
        }
        classes.pImages i = new classes.pImages();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
                {
                    using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                    {
                        classes.pImages _i = db.ImagesTable.FirstOrDefault(x => x.id == i.id);
                        classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == i.id);
                        
                        foreach (var file in app.FilePickerContinuationArgs.Files)
                        {

                            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                            await file.CopyAsync(localFolder, file.Name, NameCollisionOption.ReplaceExisting);
                           _i.Img.Add(new classes.ImageList("ms-appdata:///Local/" + file.Name, _i.id));


                        } 
                        _i.number = i.number = g.subtitle = _i.Img.Count + " photo"; i = _i;
                        db.SubmitChanges();
                    }
                    // Process file here

                    app.FilePickerContinuationArgs = null;
                }
            }
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.delete;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.addimage;
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {

                   i = db.ImagesTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();
                    LayoutRoot.DataContext = i;
                    items.ItemsSource = i.Img;
                }
            }
            base.OnNavigatedTo(e);
        }
      async  void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            App.mp = "";
            try
            {

                if (e.TaskResult == TaskResult.OK)
                {
                    var AppIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    string filename = Guid.NewGuid().ToString();
                    var FileName = AppIsolatedStorage.CreateFile(filename + ".jpg");
                    byte[] bytes = new byte[4096];
                    int Count;
                    while ((Count = e.ChosenPhoto.Read(bytes, 0, 4096)) > 0)
                    {

                        FileName.Write(bytes, 0, Count);

                    }
                    FileName.Close();
                    using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                    {
                        classes.pImages _i = db.ImagesTable.FirstOrDefault(x => x.id == i.id);
                        classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == i.id);
                        _i.Img.Add(new classes.ImageList("ms-appdata:///Local/" + filename + ".jpg",i.id));
                        
                        _i.number = i.number = g.subtitle =  _i.Img.Count + " photo";i = _i;
                        
                        db.SubmitChanges();
                    }
                    

         
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu.IsOpen = true;
        }

        private  void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(i.title);
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

                classes.pImages _i = db.ImagesTable.FirstOrDefault(x => x.id == i.id);
                i.title = _i.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == i.id);
                g.title = y.Text;
                db.SubmitChanges();
                    
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = i;
            }
        }
        classes.ImageList f  ;
        private async void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            var filename = (classes.ImageList)(((Image)sender).DataContext);
            f = filename;
            filename._name = filename._name.Replace("ms-appdata:///Local/", "");
            BitmapImage bi = new BitmapImage();
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(filename._name))
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(filename._name, FileMode.Open, FileAccess.Read))
                    {
                        bi.SetSource(fileStream);
                    }
            }
            imagepopup.Source = bi;
            await UIElementExtensions.SlideElement(pPhoto, SlideTransitionMode.SlideUpFadeIn);
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;

        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ctxmenu1.IsOpen == true)
            {
                ctxmenu1.IsOpen = false;
                e.Cancel = true;
                return;
            }
            if (pPhoto.Visibility == Visibility.Visible)
            {
                pPhoto.Visibility = Visibility.Collapsed;
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                e.Cancel = true;
                return;
            }
            base.OnBackKeyPress(e);
        }

        private  void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pImages).id).First();
               List<classes.ImageList> ii = db.imageList.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pImages).id).ToList();
                classes.pImages _i = db.ImagesTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pImages).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                db.imageList.DeleteAllOnSubmit(ii);
                db.ImagesTable.DeleteOnSubmit(_i);
                db.SubmitChanges();
                
            }
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            openPicker.PickMultipleFilesAndContinue();
        
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu1.IsOpen = true;
        }

        Microsoft.Xna.Framework.Media.MediaLibrary library = new Microsoft.Xna.Framework.Media.MediaLibrary();

        private void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            try
            {
    string  filename = f._name.Replace("ms-appdata:///Local/", "");
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(filename))
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        library.SavePicture(filename, fileStream);
                        App.ToastMessage(AppResources.saved, AppResources.imagesaved);
                    }
            }
            }
            catch (Exception ex)
            {
                
            }
      
        }
       
        private void RadContextMenuItem_Tapped_3(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            try
            {
                string filename = f._name.Replace("ms-appdata:///Local/", "");
                        ShareMediaTask shareMediaTask = new ShareMediaTask();
            shareMediaTask.FilePath = filename;
            shareMediaTask.Show();
            }
            catch (Exception ex)
            {

            }

        }

        private async void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            if (f==null) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == i.id);
                classes.ImageList il = db.ImagesTable.FirstOrDefault(x => x.id == i.id).Img.FirstOrDefault(x=>x.fid==f.fid);
                           i.Img.Remove(i.Img.Where(x=>x.fid==f.fid).FirstOrDefault());
                            
                db.imageList.DeleteOnSubmit(il);
                db.SubmitChanges();
                classes.pImages _i = db.ImagesTable.FirstOrDefault(x => x.id == i.id);
_i = i;
                g.subtitle = i.number = _i.number = (string)(i.Img.Count).ToString() + AppResources.photo;
                db.SubmitChanges();
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = i;
            
            }

            pPhoto.Visibility = Visibility.Collapsed;
            App.ToastMessage(AppResources.deleted, AppResources.imagedelete);
            f=null;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }
    }
}