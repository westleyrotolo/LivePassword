using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Security.Credentials;
using LivePassword.Resources;

namespace LivePassword.Views
{
    public partial class ChangePassword : PhoneApplicationPage
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextPassword.Text==GetCredential())
            {
                if ((!(string.IsNullOrEmpty(TextNewPassword.Text))) || (!(string.IsNullOrEmpty(TextRetypePassword.Text))))
                {
                    if (TextNewPassword.Text==TextRetypePassword.Text)
                    {
                        RemoveCredential();
                        SaveCredential(TextNewPassword.Text);
                        NavigationService.GoBack();
                    }
                    else
                    {
                        TextNewPassword.IsValid = false;
                        TextRetypePassword.IsValid = false;
                        TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                        TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                    }
                }
                else
                {
                    TextRetypePassword.IsValid = false;
                    TextNewPassword.IsValid = false;
                    TextNewPassword.ValidationText = AppResources.fieldnotempty;
                    TextRetypePassword.ValidationText = AppResources.fieldnotempty;
                }
            }
            else
            {
                TextPassword.IsValid = false;
                TextPassword.ValidationText = AppResources.passwordincorrect;
            }
        }       
        #region PasswordVault
        private void SaveCredential(string password)
        {
            var vault = new PasswordVault();
            var credential = new PasswordCredential("Live Password", "Current User", password);

            vault.Add(credential);
        
        }

        private string GetCredential()
        {
            string userName, password = null;

            var vault = new PasswordVault();
            try
            {
                var credential = vault.FindAllByResource("Live Password").FirstOrDefault();
                if (credential != null)
                {
                    userName = credential.UserName;
                    password = vault.Retrieve("Live Password", userName).Password;
                }
            }
            catch { }

            return password;
        }

        private void RemoveCredential()
        {
            var vault = new PasswordVault();
            try
            {
                vault.Remove(vault.Retrieve("Live Password", "Current User"));
            }
            catch { }
        }
        #endregion

    }
}