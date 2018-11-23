using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Phone.Shell;
using LivePassword.Resources;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Live;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Tasks;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Store;
using System.Threading.Tasks;

namespace LivePassword
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<classes.pGeneral> general = new List<classes.pGeneral>();
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            TiltEffect.TiltableItems.Add(typeof(Border));

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.add;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.managekey;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = AppResources.uploadOneDrive;

            items.Width = LayoutRoot.Width = App.Current.Host.Content.ActualWidth;
            LayoutRoot.Height = App.Current.Host.Content.ActualHeight;
            items.Height = LayoutRoot.Height - 30 - 58;
            //Shows the trial reminder message, according to the settings of the TrialReminder.
            //(App.Current as App).trialReminder.Notify();

            ////Shows the rate reminder message, according to the settings of the RateReminder.
            //(App.Current as App).rateReminder.Notify();
        }

        public ObservableCollection<StringKeyGroup<classes.pGeneral>> GroupedItems(IEnumerable<classes.pGeneral> source)
        {
            return StringKeyGroup<classes.pGeneral>.CreateGroups(source,
                System.Threading.Thread.CurrentThread.CurrentUICulture,
                s => s._type, true);
        }

        private List<Group<classes.pGeneral>> GetCityGroups()
        {

            IEnumerable<classes.pGeneral> cityList = general;
            return GetItemGroups(cityList, c => c._type);
        }

        private static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
        {
            IEnumerable<Group<T>> groupList = from item in itemList
                                              group item by getKeyFunc(item) into g
                                              orderby g.Key
                                              select new Group<T>(g.Key, g);

            return groupList.ToList();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (p1.IsOpen == true)
            {
                p1.IsOpen = false;
                e.Cancel = true;
                return;
            }
            if (pAddItem.Visibility == Visibility.Visible)
            {

                pAddItem.Visibility = Visibility.Collapsed;
                add();
                e.Cancel = true;
                ApplicationBar.IsVisible = true;
                return;
            }
            App.Current.Terminate();
            base.OnBackKeyPress(e);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnwebsite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCreditCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNote_Click(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            add();
            pAddItem.Visibility = Visibility.Collapsed;
            ApplicationBar.IsVisible = true;

            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                general = db.GeneralTable.ToList().OrderBy(x => x.title).ToList();
            }
            if (general.Count == 0)
            {
                textvisibility.Visibility = Visibility.Visible;
                items.Visibility = Visibility.Collapsed;
            }
            else
            {
                textvisibility.Visibility = Visibility.Collapsed;
                items.Visibility = Visibility.Visible;
                items.ItemsSource = GetCityGroups();
            }

            base.OnNavigatedTo(e);
        }
        void cancel()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton button = new ApplicationBarIconButton(new Uri("/Images/cancel.png", UriKind.Relative));
            button.Text = AppResources.cancel;
            button.Click += button_Click;

            ApplicationBar.Buttons.Add(button);
        }
        void add()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton button2 = new ApplicationBarIconButton(new Uri("/Images/add.png", UriKind.Relative));
            button2.Text = AppResources.add;
            button2.Click += ApplicationBarIconButton_Click;
            ApplicationBar.Buttons.Add(button2);
        }
        void button_Click(object sender, EventArgs e)
        {
            pAddItem.Visibility = Visibility.Collapsed;
            add();
        }
        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            pivot.SelectedIndex = 0;
            LicenseInformation triall = new LicenseInformation();
            if (triall.IsTrial())
            {
                if (general.Count >= 3)
                {
                    if (MessageBox.Show(AppResources.buyfullversion, AppResources.Trial, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        MarketplaceDetailTask market = new MarketplaceDetailTask();
                        market.ContentType = MarketplaceContentType.Applications;
                        market.Show();
                    }
                }
                else
                {
                    cancel();
                    await UIElementExtensions.SlideElement(pAddItem, SlideTransitionMode.SlideUpFadeIn);
                }
            }
            else
            {

                cancel();
                await UIElementExtensions.SlideElement(pAddItem, SlideTransitionMode.SlideUpFadeIn);
            }
        }

        private void brd_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            classes.pGeneral p = ((Border)sender).DataContext as classes.pGeneral;
            if (p._type == "Login")
            {
                NavigationService.Navigate(new Uri("/View/pageLogin.xaml?id=" + p.id, UriKind.Relative));

            }
            else if (p._type == "Note")
            {
                NavigationService.Navigate(new Uri("/View/pageNote.xaml?id=" + p.id, UriKind.Relative));
            }
            else if (p._type == "WebSite")
            {
                NavigationService.Navigate(new Uri("/View/pageWebSite.xaml?id=" + p.id, UriKind.Relative));
            }
            else if (p._type == "Mail")
            {
                NavigationService.Navigate(new Uri("/View/pageMail.xaml?id=" + p.id, UriKind.Relative));
            }
            else if (p._type == "CreditCard")
            {
                NavigationService.Navigate(new Uri("/View/pageCreditCard.xaml?id=" + p.id, UriKind.Relative));
            }
            else if (p._type == "Images")
            {
                NavigationService.Navigate(new Uri("/View/pagePhoto.xaml?id=" + p.id, UriKind.Relative));
            }
            else if (p._type == "files")
            {
                NavigationService.Navigate(new Uri("/View/pageFiles.xaml?id=" + p.id + "&type=" + p._type, UriKind.Relative));
            }
            else if (p._type == "audio")
            {
                NavigationService.Navigate(new Uri("/View/pageFiles.xaml?id=" + p.id + "&type=" + p._type, UriKind.Relative));
            }
            else if (p._type == "video")
            {
                NavigationService.Navigate(new Uri("/View/pageFiles.xaml?id=" + p.id + "&type=" + p._type, UriKind.Relative));
            }
            else if (p._type == "office")
            {
                NavigationService.Navigate(new Uri("/View/pageFiles.xaml?id=" + p.id + "&type=" + p._type, UriKind.Relative));
            }
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));

        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/WebSitePage.xaml", UriKind.Relative));

        }

        private void Border_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MailPage.xaml", UriKind.Relative));

        }

        private void Border_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NotePage.xaml", UriKind.Relative));


        }

        private void Border_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreditCard.xaml", UriKind.Relative));

        }

        private void Border_Tap_5(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Manage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Traduzione.xaml", UriKind.Relative));
        }
        async void allkey()
        {

            string s = "";
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                radbusy.Visibility = Visibility.Visible;
                radbusy.IsRunning = true;
                List<classes.pCreditCard> c = db.CreditCardTable.ToList();
                if (c.Count > 0)
                    s += "Credit Card\n";
                foreach (var _c in c)
                {
                    s += string.Format("Title: {0}\nNumber: {1}\nExpiration: {2}\nCVV: {3}\n", _c.title, _c.number, _c.scadenza, _c.cvc);
                }

                List<classes.pLogin> l = db.LoginTable.ToList();
                if (l.Count > 0)
                    s += "\nLogin\n";
                foreach (var _l in l)
                {
                    s += string.Format("Title: {0}\nUser: {1}\nPassword: {2}\n", _l.title, _l.user, _l.password);
                }

                List<classes.pMail> m = db.MailTable.ToList();
                if (m.Count > 0)
                    s += "\nMail\n";
                foreach (var _m in m)
                {
                    s += string.Format("Title: {0}\nSiteWeb: {1}\nMail: {2}\nPassword: {3}\n", _m.title, _m.site, _m.mail, _m.password);
                }
                List<classes.pNote> n = db.NoteTable.ToList();
                if (n.Count > 0)
                    s += "\nNote\n";
                foreach (var _n in n)
                {
                    s += string.Format("Title: {0}\nText: {1}\n", _n.title, _n.text);
                }

                List<classes.pWebSite> w = db.WebSiteTable.ToList();
                if (w.Count > 0)
                    s += "\nSiteWeb\n";
                foreach (var _w in w)
                {
                    s += string.Format("Title: {0}\nSiteWeb: {1}\nUser: {2}\nPassword: {3}\n", _w.title, _w.website, _w.user, _w.password);
                }

            }
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            //create new file
            using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream("LivePassword.txt", FileMode.Create, FileAccess.Write, myIsolatedStorage)))
            {
                await writeFile.WriteAsync(s);
            }
            radbusy.IsRunning = false;
            radbusy.Visibility = Visibility.Collapsed;

        }
        LiveConnectClient client;
        LiveConnectSession session;
        void carica()
        {
            try
            {
                if ((session != null))
                {

                    client = new LiveConnectClient(session);
                    allkey();
                    //Get files from isolated store
                    IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream fileStream = store.OpenFile("livepassword.txt", FileMode.Open);

                    //Upload files to document folder by friendly name.
                    client.UploadCompleted += new EventHandler<LiveOperationCompletedEventArgs>(client_UploadCompleted);
                    client.UploadAsync("me/skydrive/", "livepassword.txt", fileStream, OverwriteOption.Overwrite);
                    radbusy2.Visibility = Visibility.Visible;
                    radbusy2.IsRunning = true;
                }


            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
        }


        void skydrive_Click(object sender, EventArgs e)
        {

            LicenseInformation triall = new LicenseInformation();
            if (triall.IsTrial())
            {
                if (MessageBox.Show(AppResources.Trial, AppResources.Purchase, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MarketplaceDetailTask market = new MarketplaceDetailTask();
                    market.ContentType = MarketplaceContentType.Applications;
                    market.Show();
                }
            }
            else
            {
                if ((session != null))
                { carica(); }


            }
        }
        private void btn_Upload_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            try
            {
                if (e.Status == LiveConnectSessionStatus.Connected)
                {
                    session = e.Session;
                    client = new LiveConnectClient(session);
                    Dispatcher.BeginInvoke(() =>
                    {
                        radbusy2.Visibility = Visibility.Visible;
                        radbusy2.IsRunning = true;
                        carica();
                    });

                }

                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => { MessageBox.Show(e.Error.Message); p1.IsOpen = false; });
                }
            }
            catch
            {
                p1.IsOpen = false;
            }


        }
        public void prova()
        {
        }
        void client_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    IDictionary<string, object> fileInfo = e.Result;
                    string fileId = AppResources.FileSalvato;
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(fileId);
                    });
                }
                else
                {
                    string errorMessage = "Error calling API: " + Convert.ToString(e.Error.Message);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(errorMessage);
                    });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {


            }
            radbusy2.Visibility = Visibility.Collapsed;

            radbusy2.IsRunning = false;
            p1.IsOpen = false;
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            p1.IsOpen = true;
        }

        private void bfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FilePage.xaml?id=files", UriKind.Relative));
        }

        private void Border_Tap_6(object sender, System.Windows.Input.GestureEventArgs e)
        {

            NavigationService.Navigate(new Uri("/FilePage.xaml?id=audio", UriKind.Relative));
        }

        private void Border_Tap_7(object sender, System.Windows.Input.GestureEventArgs e)
        {

            NavigationService.Navigate(new Uri("/FilePage.xaml?id=office", UriKind.Relative));
        }

        private void Border_Tap_8(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FilePage.xaml?id=video", UriKind.Relative));

        }
    }

}
