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
using LivePassword.Resources;

namespace LivePassword
{
    public partial class CreditCard : PhoneApplicationPage
    {
        public CreditCard()
        {
            InitializeComponent();
            txtcvv.LostFocus += txtcvv_LostFocus;
            txttile.LostFocus += txtcvv_LostFocus;
            txtexpiration.LostFocus += txtcvv_LostFocus;
            txtnumber.LostFocus += txtcvv_LostFocus;
            txttile.KeyUp += txttile_KeyUp;
            txtnumber.KeyUp += txtnumber_KeyUp;
            txtexpiration.KeyUp += txtexpiration_KeyUp;
        }

        void txtexpiration_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                if (txtexpiration.Text.Length==2)
                {
                    txtexpiration.Text = txtexpiration.Text.Substring(0, 1);
                    txtexpiration.SelectionStart = txtexpiration.Text.Length;
                    txtexpiration.SelectionLength = 1;
                }
            }
            if (txtexpiration.Text.Length == 2)
            {
                txtexpiration.Text += "/";
                txtexpiration.SelectionStart = txtexpiration.Text.Length;
                txtexpiration.SelectionLength = 1;
            }
            if (e.Key == System.Windows.Input.Key.Enter)
                txtcvv.Focus();
        }

        void txtnumber_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtexpiration.Focus();
        }

        void txttile_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtnumber.Focus();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            txttile.Focus();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.confirm;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.cancel;

            base.OnNavigatedTo(e);
        }
        void txtcvv_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;
            if (t.Text != "")
                t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttile.Text == "") || (txtnumber.Text == "") || (txtexpiration.Text == "") || (txtcvv.Text == ""))
            {
                if (txttile.Text == "")
                    txttile.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtnumber.Text == "")
                    txtnumber.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtexpiration.Text == "")
                    txtexpiration.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtcvv.Text == "")
                    txtcvv.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);

            }
            else
            {

                classes.pCreditCard creditcard = new classes.pCreditCard();
                creditcard.id = Guid.NewGuid().ToString();
                creditcard.number = txtnumber.Text;
                creditcard.scadenza = txtexpiration.Text;
                creditcard.title = txttile.Text;
                creditcard.cvc = txtcvv.Text;
                classes.pGeneral g = new classes.pGeneral(creditcard.id, creditcard.title, creditcard.number, "CreditCard");
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    db.CreditCardTable.InsertOnSubmit(creditcard);
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

        private void txtexpiration_TextChanged(object sender, TextChangedEventArgs e)
        {
            
           

        }
    }
}