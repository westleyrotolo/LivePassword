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
using Windows.Storage;
using Windows.UI.ViewManagement;
using Microsoft.Phone.Info;
using LivePassword.Resources;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LivePassword.Views
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private bool _isRegistered;

        public LoginPage()
        {
            InitializeComponent();

        //x    Loaded += OnLoaded;

            // Si verifica uno strano behavior, evitiamo
            //InputPane.GetForCurrentView().Showing += OnInputPaneShowing;
        }

        private void OnInputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            var factor = 480 / args.OccludedRect.Width;
            if (args.OccludedRect.Height > 0)
            {
                KeyboardPlaceholder.Height = new GridLength(args.OccludedRect.Height * factor, GridUnitType.Pixel);
            }
        }

       

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                KeyboardPlaceholder.Height = new GridLength(408, GridUnitType.Pixel);
            }
            else
            {
                KeyboardPlaceholder.Height = new GridLength(339, GridUnitType.Pixel);
            }

            // Migrazione della password, forse sarebbe meglio gestirlo insieme al resto, 
            // ma non ho nessuna voglia di farlo, sono le 05:17 e me ne vado a dormire
            var p = await StorageHelper.LoadData("password.dat", typeof(LivePassword.classes.pPassword)) as LivePassword.classes.pPassword;
            if (p != null)
            {
                await Migration.MigrationWizard.Migrate();
            }

            _isRegistered = GetCredential() != null;

            RegisterPanel.Visibility = _isRegistered ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            LoginPanel.Visibility = _isRegistered ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (_isRegistered)
            {
                this.Loaded += ((_s,_e) =>
                    {
                        TextPassword.Focus();
                    });

            }
            else
                Presentation.Visibility = System.Windows.Visibility.Visible;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (TextPassword.Text == GetCredential())
            {
                App.IsLoggedIn = true;
                NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                TextPassword.IsValid = true;
                TextPassword.IsValid = false;
                TextPassword.ValidationText = AppResources.passwordincorrect;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextNewPassword.Text))
            {
                if (!string.IsNullOrWhiteSpace(TextRetypePassword.Text))
                {
                    if (TextNewPassword.Text == TextRetypePassword.Text)
                    {
                        App.IsLoggedIn = true;
                        RemoveCredential();
                        SaveCredential(TextNewPassword.Text);
                        NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        TextNewPassword.IsValid = true;
                        TextNewPassword.IsValid = false;
                        TextRetypePassword.IsValid = true;
                        TextRetypePassword.IsValid = false;
                        TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                        TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                    }
                }
                else
                {
                    TextNewPassword.IsValid = true;
                    TextNewPassword.IsValid = false;
                    TextRetypePassword.IsValid = true;
                    TextRetypePassword.IsValid = false;
                    TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                    TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                }
            }
            else
            {
                TextNewPassword.IsValid = true;
                TextNewPassword.IsValid = false;
                TextRetypePassword.IsValid = true;
                TextRetypePassword.IsValid = false;
                TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
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

        private  void RemoveCredential()
        {
            var vault = new PasswordVault();
            try
            {
                vault.Remove(vault.Retrieve("Live Password", "Current User"));
            }
            catch { }
        }
        #endregion



        private void Login_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
     if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (TextPassword.Text == GetCredential())
                {
                    App.IsLoggedIn = true;
                    NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                }
                else
                {
                    TextPassword.IsValid = true;
                    TextPassword.IsValid = false;
                    TextPassword.ValidationText = AppResources.passwordincorrect;
                }
            }
        }

        private void TextRetypePassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
     if (e.Key== System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(TextNewPassword.Text))
                {
                    if (!string.IsNullOrWhiteSpace(TextRetypePassword.Text))
                    {
                        if (TextNewPassword.Text == TextRetypePassword.Text)
                        {
                            App.IsLoggedIn = true;
                            RemoveCredential();
                            SaveCredential(TextNewPassword.Text);
                            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                        }
                        else
                        {

                            TextNewPassword.IsValid = true;
                            TextNewPassword.IsValid = false;
                            TextRetypePassword.IsValid = true;
                            TextRetypePassword.IsValid = false;
                            TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                            TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                        }
                    }
                    else
                    {
                        TextNewPassword.IsValid = true;
                        TextNewPassword.IsValid = false;
                        TextRetypePassword.IsValid = true;
                        TextRetypePassword.IsValid = false;
                        TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                        TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                    }
                }
                else
                {
                    TextNewPassword.IsValid = true;
                    TextNewPassword.IsValid = false;
                    TextRetypePassword.IsValid = true;
                    TextRetypePassword.IsValid = false;
                    TextNewPassword.ValidationText = AppResources.passwordnotmatch;
                    TextRetypePassword.ValidationText = AppResources.passwordnotmatch;
                }
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            _isRegistered = false;
            RegisterPanel.Visibility = _isRegistered ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            LoginPanel.Visibility = _isRegistered ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            RemoveCredential();
            TextNewPassword.Focus();
            Presentation.Visibility = Visibility.Collapsed;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e1.Fill = e2.Fill = e3.Fill = e4.Fill = e5.Fill = e6.Fill = new SolidColorBrush(Colors.Transparent);
            ListBox l = sender as ListBox;
           ((Ellipse)l.Items[pivot.SelectedIndex]).Fill = new SolidColorBrush(Colors.White);
         
        }
    }
}