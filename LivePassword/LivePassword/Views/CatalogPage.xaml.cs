using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LivePassword.ViewModels;
using LivePassword.Entities;
using LivePassword.Common;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using LivePassword.Resources;

namespace LivePassword.Views
{
    public partial class CatalogPage : PhoneApplicationPage
    {
        public CatalogPage()
        {
            InitializeComponent();
        }
        bool productStatus;
      
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                (DataContext as CatalogViewModel).Initialize();
            }
            productStatus = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses["LivePasswordPro"].IsActive;

        }
        public async Task<bool> LivePasswordProfessional()
        {
            try
            {
                await CurrentApp.RequestProductPurchaseAsync("LivePasswordPro", false);
                if (CurrentApp.LicenseInformation.ProductLicenses.ContainsKey("LivePasswordPro"))
                {
                    ProductLicense license = CurrentApp.LicenseInformation.ProductLicenses["LivePasswordPro"];
                    if (license.IsActive)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as LongListSelector;
            var item = list.SelectedItem as ICatalogItem;
            list.SelectedItem = null;
            if (item != null)
            {
                if (item is Credential)
                {
                    NavigationService.Navigate("/Views/CredentialAddPage.xaml", item);
                }
                else if (item is Folder)
                {
                    if (!(productStatus))
                    {

                        if (MessageBox.Show(AppResources.buyfullversion, AppResources.LivePasswordPro, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            productStatus = await LivePasswordProfessional();
                        }

                        return;
                    }
                    NavigationService.Navigate("/Views/FolderAddPage.xaml", item);
                }

                list.SelectedItem = null;
            }
        }
    }
}