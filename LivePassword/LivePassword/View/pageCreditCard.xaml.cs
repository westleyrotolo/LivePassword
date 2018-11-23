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
    public partial class pageCreditCard : PhoneApplicationPage
    {
        public pageCreditCard()
        {
            InitializeComponent();
        }
        classes.pCreditCard c = new classes.pCreditCard();
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


                    c = db.CreditCardTable.Where(x => x.id == NavigationContext.QueryString["id"]).FirstOrDefault();
                    LayoutRoot.DataContext = c;
                }
            }
            base.OnNavigatedTo(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);

        }
        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = AppResources.creditcard;
            requestData.Properties.Description = AppResources.sharecreditcard;
            string text = string.Format("\nTitle: {0}\nNumber: {1}\nExpiration: {2}\nCVV: {3}\nLive Password on Windows Phone", c.title, c.number, c.scadenza, c.cvc);
            requestData.SetText(text);
        }
        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                classes.pGeneral g = db.GeneralTable.Where(x=>x.id == ((LayoutRoot.DataContext) as classes.pCreditCard).id).First();
                db.GeneralTable.DeleteOnSubmit(g);
                classes.pCreditCard c = db.CreditCardTable.Where(x => x.id == ((LayoutRoot.DataContext) as classes.pCreditCard).id).First();
                db.CreditCardTable.DeleteOnSubmit(c);
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
            Clipboard.SetText(c.title);
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

                classes.pCreditCard _c = db.CreditCardTable.FirstOrDefault(x => x.id == c.id);
                c.title = _c.title = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == c.id);
                g.title = y.Text;
                db.SubmitChanges();

            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = c;
        }

        private void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu1.IsOpen = true;
        }

        private void RadContextMenuItem_Tapped_2(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(c.number);
            App.ToastMessage(AppResources.copied, AppResources.numbercopied);
        }

        private async void RadContextMenuItem_Tapped_3(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifynumer;
            string message = AppResources.number;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pCreditCard _c = db.CreditCardTable.FirstOrDefault(x => x.id == c.id);
                c.number = _c.number = y.Text;
                classes.pGeneral g = db.GeneralTable.FirstOrDefault(x => x.id == c.id);
                g.subtitle = y.Text;
                db.SubmitChanges();
                    
            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = c;
        }

        private void Border_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu3.IsOpen = true;
        }

        private void Border_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ctxmenu4.IsOpen = true;
        }

        private  void RadContextMenuItem_Tapped_4(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {    
            Clipboard.SetText(c.scadenza);
            App.ToastMessage(AppResources.copied, AppResources.expirationcopied);
        }

        private async void RadContextMenuItem_Tapped_5(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifyexpiration;
            string message = AppResources.expiration;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pCreditCard _c = db.CreditCardTable.FirstOrDefault(x => x.id == c.id);
                c.scadenza = _c.scadenza = y.Text;
               
                db.SubmitChanges();
                    
            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = c;
        }

        private void RadContextMenuItem_Tapped_6(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            Clipboard.SetText(c.cvc);
            App.ToastMessage(AppResources.copied, AppResources.cvvcopied);
        }

        private async void RadContextMenuItem_Tapped_7(object sender, Telerik.Windows.Controls.ContextMenuItemSelectedEventArgs e)
        {
            string title = AppResources.modifyvv;
            string message = AppResources.cvv;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {

                classes.pCreditCard _c = db.CreditCardTable.FirstOrDefault(x => x.id == c.id);
                c.cvc = _c.cvc = y.Text;
                db.SubmitChanges();
                    
            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = c;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {

            DataTransferManager.ShowShareUI();

        }
    }
}