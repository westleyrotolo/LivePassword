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
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            txtpassword.LostFocus += txtpassword_LostFocus;
            txttitle.LostFocus += txtpassword_LostFocus;
            txtuser.LostFocus += txtpassword_LostFocus;
            txttitle.KeyUp += txttitle_KeyUp;
            txtuser.KeyUp += txtuser_KeyUp;
            txtpassword.KeyUp += txtpassword_KeyUp;
        }

        void txtpassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txttitle.Focus();
        }

        void txtuser_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtpassword.Focus();
        }

        void txttitle_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            txtuser.Focus();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            txttitle.Focus();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.confirm;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.cancel;
            base.OnNavigatedTo(e);
        }
        void txtpassword_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;
            if (t.Text != "")
                t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");
     
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttitle.Text == "") || (txtuser.Text == "") || (txtpassword.Text == ""))
            {
                if (txttitle.Text == "")
                    txttitle.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtuser.Text == "")
                    txtuser.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtpassword.Text == "")
                    txtpassword.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);

            }
            else
            {

                classes.pLogin login = new classes.pLogin();
                login.id = Guid.NewGuid().ToString();
                login.user = txtuser.Text;
                login.title = txttitle.Text;
                login.password = txtpassword.Text;
                classes.pGeneral g = new classes.pGeneral(login.id, login.title, login.user, "Login");
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    db.LoginTable.InsertOnSubmit(login);
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
    }
}