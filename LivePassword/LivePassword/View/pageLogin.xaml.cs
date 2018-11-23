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
    public partial class pageLogin : PhoneApplicationPage
    {
        public pageLogin()
        {
            InitializeComponent();
        }
        classes.pLogin l = new classes.pLogin();
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
                l = db.LoginTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();
                LayoutRoot.DataContext = l;

                }
               
            }
            base.OnNavigatedTo(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);
   
        }
        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = AppResources.login;
            requestData.Properties.Description = AppResources.sharelogin;
            string text = string.Format("\nTitle: {0}\nUser: {1}\nPassword: {2}\nLive Password on Windows Phone", l.title, l.user, l.password);
            requestData.SetText(text);
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {

            DataTransferManager.ShowShareUI();

        }
  

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu.IsOpen = true;
        }


        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pLogin).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                classes.pLogin c = db.LoginTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pLogin).id).First();
                db.LoginTable.DeleteOnSubmit(c);
                db.SubmitChanges();
            }
            NavigationService.GoBack();
        }
      private void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(l.password);
            App.ToastMessage(AppResources.copied, AppResources.passwordcopied);
        }

        private async void RadContextMenuItem_Tapped_1(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifypassword;
            string message = AppResources.password;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title,MessageBoxButtons.OKCancel,message,InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pLogin _l = db.LoginTable.FirstOrDefault(x => x.id == l.id);
                l.password = _l.password = y.Text;
                db.SubmitChanges();

            }
           LayoutRoot.DataContext = null;
           LayoutRoot.DataContext = l;
        }
   
        private void RadContextMenuItem_Tapped_2(object sender, ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(l.user);
            App.ToastMessage(AppResources.copied, AppResources.usercopied);
        }

        private async void RadContextMenuItem_Tapped_3(object sender, ContextMenuItemSelectedEventArgs e)
        {

            string title = AppResources.modifyuser;
            string message = AppResources.user;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pLogin _l = db.LoginTable.FirstOrDefault(x => x.id == l.id);
                l.user = _l.user = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == l.id);
                g.subtitle = y.Text;
                db.SubmitChanges();

            }    
           
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = l;
        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu2.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_4(object sender, ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(l.title);
            App.ToastMessage(AppResources.copied,AppResources.titlecopied);
        }

        private async void RadContextMenuItem_Tapped_5(object sender, ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modfytitle;
            string message = AppResources.title;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pLogin _l = db.LoginTable.FirstOrDefault(x => x.id == l.id);
                l.title = _l.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == l.id);
                g.title = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = l;
        }

        private void Border_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu3.IsOpen = true;
        }
    }
}