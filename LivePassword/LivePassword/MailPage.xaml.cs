using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Globalization;
using Telerik.Windows.Controls;
using LivePassword.Resources;

namespace LivePassword
{
    public partial class MailPage : PhoneApplicationPage
    {
        public MailPage()
        {
            InitializeComponent();
            txttile.LostFocus += txtwebsite_LostFocus;
            txtmail.LostFocus += txtwebsite_LostFocus;
            txtwebsite.LostFocus += txtwebsite_LostFocus;
            txtpassword.LostFocus += txtwebsite_LostFocus;
            txttile.KeyUp += txttile_KeyUp;
            txtmail.KeyUp += txtmail_KeyUp;
            txtwebsite.KeyUp += txtwebsite_KeyUp;
            txtpassword.KeyUp += txtpassword_KeyUp;

        }

        void txtpassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txttile.Focus();
        }

        void txtwebsite_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtmail.Focus();
        }

        void txtmail_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtpassword.Focus();
        }

        void txttile_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtwebsite.Focus();
        }

        

        void txtwebsite_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;
                 if (t.Text == "https://")
                t.Text = "";
            if (t.Text != "")
                t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");

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

            txtmail.SuggestionsSource = App.suggestions;
            base.OnNavigatedTo(e);
        }
        
        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttile.Text == "") || (txtwebsite.Text == "") || (txtmail.Text == "") || (txtpassword.Text == ""))
            {
                if (txttile.Text == "")
                    txttile.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtwebsite.Text == "")
                    txtwebsite.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtmail.Text == "")
                    txtmail.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtpassword.Text == "")
                    txtpassword.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);

            }
            else
            {

                classes.pMail mail = new classes.pMail();
                mail.id = Guid.NewGuid().ToString();
                mail.title = txttile.Text;
                mail.site = txtwebsite.Text;
                mail.mail = txtmail.Text;
                mail.password = txtpassword.Text;
                classes.pGeneral g = new classes.pGeneral(mail.id, mail.title, mail.mail, "Mail");
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    db.MailTable.InsertOnSubmit(mail);
                    db.GeneralTable.InsertOnSubmit(g);
                    db.SubmitChanges();
                }
                if (!App.suggestions.Contains(mail.mail))
                {
                    App.suggestions.Add(mail.mail);
                    await StorageHelper.SaveData("suggestion.dat", App.suggestions);
                }
                NavigationService.GoBack();
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

  
        private void txtwebsite_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtwebsite.Text == "")
            {
                txtwebsite.Text = "https://";
                if ((!txttile.Text.Contains(" ")) && (txttile.Text.Length > 0))
                {
                    txtwebsite.Text += txttile.Text + ".com";
                    txtwebsite.SelectionStart = 0;
                    txtwebsite.SelectionLength = txtwebsite.Text.Length;
                }
                   
                else
                {
                    txtwebsite.SelectionStart = txtwebsite.Text.Length;
                    txtwebsite.SelectionLength = txtwebsite.Text.Length+1;
                }
                txtwebsite.LostFocus+=txtwebsite_LostFocus;
            }
        }
    }
  
}