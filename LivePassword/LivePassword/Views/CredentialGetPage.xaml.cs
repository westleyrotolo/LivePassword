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
using Windows.ApplicationModel.DataTransfer;
using System.Text;
using LivePassword.Resources;
using LivePassword.Common;
using Windows.System;

namespace LivePassword.Views
{
    public partial class CredentialGetPage : PhoneApplicationPage
    {
        public CredentialViewModel ViewModel
        {
            get
            {
                return DataContext as CredentialViewModel;
            }
        }

        public CredentialGetPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AppSettings.Instance.IsPasswordHidden = false;
            if (e.NavigationMode == NavigationMode.New)
            {
                ViewModel.InitializeGet(NavigationContext.QueryString["args"]);
            }

            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var builder = new StringBuilder();
            foreach (var field in ViewModel.Fields)
            {
                builder.AppendFormat("\r\n{0}: {1}", AppResources.ResourceManager.GetString(field.DisplayName), field.Value);
            }
            builder.Append("\r\nLive Password for Windows");

            args.Request.Data.Properties.Title = "Share Credential";
            args.Request.Data.Properties.Description = AppResources.sharecreditcard;
            args.Request.Data.SetText(builder.ToString());
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
#warning Bisogna aggiungere le localizzazioni
      
            var result = await DialogService.ShowMessageAsync("Live Password", AppResources.deletestring, AppResources.delete, AppResources.cancel);
            if (result == DialogResult.Primary)
            {
                Sqlite.Database database;
                Sqlite.Statement statement;

                Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

                Sqlite.Sqlite3.sqlite3_prepare_v2(database, "DELETE FROM DataCredentialSummary WHERE Id = " + ViewModel.IdSummary, out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);

                Sqlite.Sqlite3.sqlite3_prepare_v2(database, "DELETE FROM " + ViewModel.Table + " WHERE Id = " + ViewModel.ItemId, out statement);
                Sqlite.Sqlite3.sqlite3_step(statement);
                Sqlite.Sqlite3.sqlite3_finalize(statement);

                Sqlite.Sqlite3.sqlite3_close(database);

                App.MergeAvailable = true;
                NavigationService.GoBack();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var item = button.DataContext as FieldViewModel;

            if (item != null)
            {
                Clipboard.SetText(item.Value);
                App.ToastMessage(string.Empty, AppResources.copied);
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var item = button.DataContext as FieldViewModel;

            if (item != null)
            {
#warning Bisogna aggiungere le localizzazioni

                var result = await DialogService.ShowInputAsync("Live Password", "Enter new value for " + AppResources.ResourceManager.GetString(item.DisplayName), item.Value);
                if (result != null && result != item.Value)
                {
                    item.Value = result;

                    Sqlite.Database database;
                    Sqlite.Statement statement;

                    Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

                    if (item.ColumnName == "Title" || item.ColumnName == ViewModel.SummaryKey)
                    {
                        var column = "Title";
                        if (item.ColumnName == ViewModel.SummaryKey)
                            column = "Subtitle";

                        Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE DataCredentialSummary SET {0} = '{1}' WHERE Id = {2}", column, item.Value, ViewModel.IdSummary), out statement);
                        Sqlite.Sqlite3.sqlite3_step(statement);
                        Sqlite.Sqlite3.sqlite3_finalize(statement);
                    }

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("UPDATE {0} SET {1} = '{2}' WHERE Id = {3}", ViewModel.Table, item.ColumnName, item.Value.Replace("'", "''"), ViewModel.ItemId), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    Sqlite.Sqlite3.sqlite3_close(database);
                    App.MergeAvailable = true;
                }
            }
        }

        private async void GoWebSite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var item = button.DataContext as FieldViewModel;

            if (item != null)
            {
                Uri url;
                if (Uri.TryCreate(item.Value, UriKind.Absolute, out url))
                {
                    await Launcher.LaunchUriAsync(url);
                }
            }
        }
    }
}