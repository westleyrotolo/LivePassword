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
    public partial class WebSitePage : PhoneApplicationPage
    {
        public WebSitePage()
        {
            InitializeComponent();
            txttitle.LostFocus += txttitle_LostFocus;
            txtsite.LostFocus += txttitle_LostFocus;
            txtpassword.LostFocus += txttitle_LostFocus;
            txtmail.LostFocus += txttitle_LostFocus;
            txttitle.KeyUp += txttitle_KeyUp;
            txtsite.KeyUp += txtsite_KeyUp;
            txtmail.KeyUp += txtmail_KeyUp;
            txtpassword.KeyUp += txtpassword_KeyUp;
        }

        void txtpassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txttitle.Focus();
        }



        void txtmail_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtpassword.Focus();
     
        }

        void txtsite_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtmail.Focus();

        }

        void txttitle_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtsite.Focus();
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
            txtmail.SuggestionsSource = App.suggestions;
            base.OnNavigatedTo(e);
        }
        void txttitle_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;     
            if (t.Text == "https://")
                t.Text = "";
            if (t.Text !="")
            t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttitle.Text == "") || (txtsite.Text == "") || (txtmail.Text == "") || (txtpassword.Text == ""))
            {
                if (txttitle.Text=="")
                    txttitle.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtsite.Text=="")
                    txtsite.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtmail.Text=="")
                    txtmail.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtpassword.Text == "")
                    txtpassword.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                        
            }
            else
            {
                classes.pWebSite website = new classes.pWebSite();
                website.id = Guid.NewGuid().ToString();
                website.password = txtpassword.Text;
                website.title = txttitle.Text;
                website.user = txtmail.Text;
                website.website = txtsite.Text;
                classes.pGeneral g = new classes.pGeneral(website.id, website.title, website.user, "WebSite");
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    db.WebSiteTable.InsertOnSubmit(website);
                    db.GeneralTable.InsertOnSubmit(g);
                    db.SubmitChanges();
                }
                if (!App.suggestions.Contains(website.user))
                {
                    App.suggestions.Add(website.user);
                    await StorageHelper.SaveData("suggestion.dat", App.suggestions);
                }
                    NavigationService.GoBack();
            }
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void txtsite_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtsite.Text == "")
            {
                txtsite.Text = "https://";
                if ((!txttitle.Text.Contains(" ")) && (txttitle.Text.Length > 0))
                {
                    txtsite.Text += txttitle.Text + ".com";
                    txtsite.SelectionStart = 0;
                    txtsite.SelectionLength = txtsite.Text.Length;
                }
                else
                {
                    txtsite.SelectionStart = txtsite.Text.Length;
                    txtsite.SelectionLength = txtsite.Text.Length+1;
                }
            }
        }

     
        
    }
}