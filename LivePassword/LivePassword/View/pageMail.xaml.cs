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
    public partial class pageMail : PhoneApplicationPage
    {
        public pageMail()
        {
            InitializeComponent();
        }
        classes.pMail m = new classes.pMail(); 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.delete;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.shares;
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    m = db.MailTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();

                    LayoutRoot.DataContext = m;
                }
            }
            base.OnNavigatedTo(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);

        }
        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = AppResources.mail;
            requestData.Properties.Description = AppResources.sharemail;
            string text = string.Format("\nTitle: {0}\nSite: {1}\nMail: {2}\nPassword: {3}\nLive Password on Windows Phone", m.title, m.site, m.mail, m.password);
            requestData.SetText(text);
        }
        private  void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pMail).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                classes.pMail c = db.MailTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pMail).id).First();
                db.MailTable.DeleteOnSubmit(c);
                db.SubmitChanges();
            }
            NavigationService.GoBack();
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(m.title);
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

                classes.pMail _l = db.MailTable.FirstOrDefault(x => x.id == m.id);
                m.title = _l.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == m.id);
                g.title = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = m;
        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu1.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(m.site);
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

                classes.pMail _l = db.MailTable.FirstOrDefault(x => x.id ==m.id);
                m.site = _l.site = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = m;


        }

        private void Border_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu2.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(m.mail);
            App.ToastMessage(AppResources.mail, AppResources.mailcopied);
        }

        private async void RadContextMenuItem_Tapped_5(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifymail;
            string message = AppResources.mail;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;

            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pMail _c = db.MailTable.FirstOrDefault(x => x.id == m.id);
                m.mail = _c.mail = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == m.id);
                g.subtitle = y.Text;
                db.SubmitChanges();

            } 
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = m;
        }

        private void Border_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu3.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_6(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(m.password);
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

                classes.pMail _c = db.MailTable.FirstOrDefault(x => x.id == m.id);
                m.password = _c.password = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = m;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void RadContextMenuItem_Tapped_8(object sender, ContextMenuItemSelectedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri= new Uri(m.site, UriKind.Absolute);
            task.Show();
        }
    }
}