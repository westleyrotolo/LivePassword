using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using LivePassword.Resources;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;

namespace LivePassword
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        public PhotoPage()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            txttitle.LostFocus += txttitle_LostFocus;
                   items.ItemsSource = img;
             }
        
        void txttitle_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;
            if (t.Text != "")
                t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");
        
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if (app.FilePickerContinuationArgs.Files != null && app.FilePickerContinuationArgs.Files.Count > 0)
                {
                    foreach (var file in app.FilePickerContinuationArgs.Files)
                    {

                        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                       await file.CopyAsync(localFolder, file.Name, NameCollisionOption.ReplaceExisting);
                       img.Add("ms-appdata:///Local/" + file.Name );
     

                    }
                    // Process file here

                    app.FilePickerContinuationArgs = null;
                }
            }

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.confirm;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.cancel;

base.OnNavigatedTo(e);
        }
        ObservableCollection<string> img = new ObservableCollection<string>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
        void photoChooserTask_Completed(object sender, PhotoResult e)
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
                    img.Add("ms-appdata:///Local/" + filename + ".jpg");
     
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttitle.Text == ""))
            {
                if (txttitle.Text == "")
                    txttitle.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);

            }
            else
            {

                classes.pImages images = new classes.pImages();
                images.title = txttitle.Text;
                images.number = img.Count.ToString() + AppResources.photo;
                
                images.id = Guid.NewGuid().ToString();
                foreach (string s in img)
                {
                    images.Img.Add(new classes.ImageList(s,images.id));
                }
                classes.pGeneral g = new classes.pGeneral(images.id, images.title, images.number, "Images");
                  using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    
                    db.ImagesTable.InsertOnSubmit(images);
                    db.GeneralTable.InsertOnSubmit(g);
                    db.SubmitChanges();
                }
                NavigationService.GoBack();
            }
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            openPicker.PickMultipleFilesAndContinue();
                
            //photoChooserTask.Show();
        }
    }
}