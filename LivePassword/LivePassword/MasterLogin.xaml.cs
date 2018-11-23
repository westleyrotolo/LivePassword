using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LivePassword.classes;
using Telerik.Windows.Controls.PhoneTextBox;
using LivePassword.Resources;

namespace LivePassword
{
    public partial class MasterLogin : PhoneApplicationPage
    {
        public MasterLogin()
        {
            
            InitializeComponent();

            App.rateReminder.Notify();
            TiltEffect.TiltableItems.Add(typeof(Border));

            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.about;
            using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
            {
                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();
                }
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ContentPanel.Visibility = Visibility.Visible;
            ppass.Password = "";
            var p = await StorageHelper.LoadData("password.dat", typeof(pPassword));
            if (p != null)
            {
                if (ApplicationBar.MenuItems.Count < 2)
                {
                    App.password = p as pPassword;
                    ApplicationBar.IsMenuEnabled = true;
                    ApplicationBarMenuItem menu = new ApplicationBarMenuItem(AppResources.changepassword);
                    menu.IsEnabled = true;
                    menu.Click += menu_Click;
                    ApplicationBar.MenuItems.Add(menu);
                }
            }
            else
            {
                ContentPanel.Visibility = Visibility.Collapsed;
                ContentPassword.Visibility = Visibility.Visible;
            }

            base.OnNavigatedTo(e);
        }

        void menu_Click(object sender, EventArgs e)
        {
            prepeat.Password = "";
            prepeatreset.Password = "";
            ContentPanel.Visibility = Visibility.Collapsed;
            ContentChangePassword.Visibility = Visibility.Visible;
        }
        protected async override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ContentChangePassword.Visibility == System.Windows.Visibility.Visible)
            {
                ppass.Password = "";
                ContentChangePassword.Visibility = Visibility.Collapsed;
                await UIElementExtensions.SlideElement(ContentPanel, SlideTransitionMode.SlideUpFadeIn);
                e.Cancel = true;
                return;
            }
            else
            {
                App.Current.Terminate();
            }
            base.OnBackKeyPress(e);
        }

        private async void Border_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.IsLoggedIn = true;

            if (pinsert.Password.ToString() == prepeat.Password.ToString())
            {


                if (pinsert.Password.ToString()=="")
                {
                    pinsert.ChangeValidationState(ValidationState.Invalid, AppResources.fieldnotempty);
                    prepeat.ChangeValidationState(ValidationState.Invalid, AppResources.fieldnotempty);
                    return;
                }
                App.password.password = pinsert.Password.ToString();
                await StorageHelper.SaveData("password.dat", App.password);
                //ContentPassword.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        
        //        await UIElementExtensions.SlideElement(ContentPanel, SlideTransitionMode.SlideUpFadeIn);
            }
            else
            {
                pinsert.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                prepeat.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                 
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml",UriKind.Relative));
        }

        private void ppass_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        
        }

        private void ppass_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (App.password.password.ToString() == ppass.Password.ToString())
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else
                {
                    ppass.ChangeValidationState(ValidationState.Invalid, AppResources.passwordincorrect);

                }
            }
        }

        private void pactualreset_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                pinsertreset.Focus();
            }
        }

        private void pinsertreset_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                prepeatreset.Focus();
            }
        }

        private async void prepeatreset_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (pactualreset.Password.ToString() == App.password.password)
                {
                    if (pinsertreset.Password.ToString() == prepeatreset.Password.ToString())
                    {
                        if (pinsert.ToString().Length == 0) return;
                        App.password.password = pinsertreset.Password.ToString();
                        await StorageHelper.SaveData("password.dat", App.password);
                        ContentChangePassword.Visibility = Visibility.Collapsed;
                        await UIElementExtensions.SlideElement(ContentPanel, SlideTransitionMode.SlideUpFadeIn);
                    }
                    else
                    {
                        pinsertreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                        prepeatreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                    }

                }
                else
                {
                    pactualreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                }
            }
        }

        private void pinsert_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                prepeat.Focus();
            }
        }

        private async void prepeat_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (pinsert.Password.ToString() == prepeat.Password.ToString())
                {
                    if (pinsert.Password.ToString() == "")
                    {
                        pinsert.ChangeValidationState(ValidationState.Invalid, AppResources.fieldnotempty);
                        prepeat.ChangeValidationState(ValidationState.Invalid, AppResources.fieldnotempty);
                        return;
                    }
                    App.password.password = pinsert.Password.ToString();
                    await StorageHelper.SaveData("password.dat", App.password);
                    //ContentPassword.Visibility = Visibility.Collapsed;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                    //        await UIElementExtensions.SlideElement(ContentPanel, SlideTransitionMode.SlideUpFadeIn);
                }
                else
                {
                    pinsert.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                    prepeat.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);

                }
            }
        }

        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (pactualreset.Password.ToString() == App.password.password)
            {
                if (pinsertreset.Password.ToString() == prepeatreset.Password.ToString())
                {
                    if (pinsert.ToString().Length == 0) return;
                    App.password.password = pinsertreset.Password.ToString();
                    await StorageHelper.SaveData("password.dat", App.password);
                    ContentChangePassword.Visibility = Visibility.Collapsed;
                    await UIElementExtensions.SlideElement(ContentPanel, SlideTransitionMode.SlideUpFadeIn);
                }
                else
                {
                    pinsertreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                    prepeatreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
                }

            }
            else
            {
                pactualreset.ChangeValidationState(ValidationState.Invalid, AppResources.passwordnotmatch);
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            App.IsLoggedIn = true;

            if (App.password.password.ToString() == ppass.Password.ToString())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                ppass.ChangeValidationState(ValidationState.Invalid, AppResources.passwordincorrect);
            }
        }
    }

}