
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using LivePassword.Resources;

namespace LivePassword.View
{
    public partial class pageWebSite : PhoneApplicationPage
    {
        public pageWebSite()
        {
            InitializeComponent();
        }
        classes.pWebSite w = new classes.pWebSite();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.delete;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.shares;
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {

             
                w = db.WebSiteTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();
                LayoutRoot.DataContext = w;
                }
            }
            base.OnNavigatedTo(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);

        }
        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = AppResources.website;
            requestData.Properties.Description = AppResources.sharesite;
            string text = string.Format("\nTitle: {0}\nSite: {1}\nUser: {2}\nPassword: {3}\nLive Password on Windows Phone", w.title, w.website, w.user, w.password);
            requestData.SetText(text);
        }
        private  void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pWebSite).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                classes.pWebSite c = db.WebSiteTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pWebSite).id).First();
                db.WebSiteTable.DeleteOnSubmit(c);
                db.SubmitChanges();
            }
          
            NavigationService.GoBack();
        }

        private void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(w.title);
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

                classes.pWebSite _c = db.WebSiteTable.FirstOrDefault(x => x.id == w.id);
                w.title = _c.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == w.id);
                g.title = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = w;
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu.IsOpen = true;
        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu1.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(w.website);
            App.ToastMessage(AppResources.copied, AppResources.sitecopied);
        }

        private async void RadContextMenuItem_Tapped_3(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifyweb;
            string message = AppResources.website;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pWebSite _c = db.WebSiteTable.FirstOrDefault(x => x.id == w.id);
                w.website = _c.website = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = w;
        }

        private void Border_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu2.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(w.user);
            App.ToastMessage(AppResources.copied, AppResources.usercopied);
        }

        private async void RadContextMenuItem_Tapped_5(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifyuser;
            string message = AppResources.user;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pWebSite _c = db.WebSiteTable.FirstOrDefault(x => x.id == w.id);
                w.user = _c.user = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == w.id);
                g.subtitle = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = w;
        }

        private void Border_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu3.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_6(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(w.password);
            App.ToastMessage(AppResources.copied, AppResources.passwordcopied);
        }

        private async void RadContextMenuItem_Tapped_7(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifypassword;
            string message = AppResources.password;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pWebSite _c = db.WebSiteTable.FirstOrDefault(x => x.id == w.id);
                w.password = _c.password = y.Text;

                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = w;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void RadContextMenuItem_Tapped_8(object sender, ContextMenuItemSelectedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri= new Uri(w.website, UriKind.Absolute);
            task.Show();
        }
    }
}