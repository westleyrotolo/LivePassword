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
    public partial class Manage : PhoneApplicationPage
    {
        public Manage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.clearall;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.add;
            items.ItemsSource = null;
            items.ItemsSource = App.suggestions;
            base.OnNavigatedTo(e);
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            App.suggestions.Clear();
            await StorageHelper.SaveData("suggestion.dat", App.suggestions);
        }

        private async void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string keyword = (string)((Border)sender).DataContext;
            App.suggestions.Remove(keyword);
            await StorageHelper.SaveData("suggestion.dat", App.suggestions);
        }

        private async void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            string title = AppResources.newkeywords;
            string message = AppResources.keyword;
            InputPromptClosedEventArgs y = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, InputMode.Text);
            if ((y.Text == "") || (y.Text == null)) return;
            if (!App.suggestions.Contains(y.Text))
            {
                App.suggestions.Add(y.Text);
                await StorageHelper.SaveData("suggestion.dat", App.suggestions);
            }
        }
           
    }
}