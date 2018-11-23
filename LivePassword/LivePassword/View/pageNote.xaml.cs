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
    public partial class pageNote : PhoneApplicationPage
    {
        public pageNote()
        {
            InitializeComponent();
        }
        classes.pNote n = new classes.pNote();
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
                    n = db.NoteTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();
                
                LayoutRoot.DataContext = n;
                }
            }
            base.OnNavigatedTo(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);

        }
        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = AppResources.note;
            requestData.Properties.Description = AppResources.sharenote;
            string text = string.Format("\nTitle: {0}\nText: {1}\nLive Password on Windows Phone", n.title, n.text);
            requestData.SetText(text);
        }
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pNote).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                classes.pNote c = db.NoteTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pNote).id).First();
                db.NoteTable.DeleteOnSubmit(c);
                db.SubmitChanges();
            }
            NavigationService.GoBack();
        }

        private void RadContextMenuItem_Tapped(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(n.title);
            App.ToastMessage(AppResources.title, AppResources.titlecopied );
        }

        private async void RadContextMenuItem_Tapped_1(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modfytitle;
            string message = AppResources.title;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pNote _c = db.NoteTable.FirstOrDefault(x => x.id == n.id);
                n.title = _c.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == n.id);
                g.title = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = n;
        }

        private void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(n.text);
                  App.ToastMessage(AppResources.copied, AppResources.textcopied);
        }

        private async void RadContextMenuItem_Tapped_3(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifytext;
            string message = AppResources.Text;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            string t;
            if (y.Text.Length >= 50)
                t = y.Text.Substring(0, 49) + "[...]";
            else
                t = y.Text;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pNote _c = db.NoteTable.FirstOrDefault(x => x.id == n.id);
                n.text = _c.text = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == n.id);
                g.subtitle = t;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = n;
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu.IsOpen = true;
        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu1.IsOpen = true;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}