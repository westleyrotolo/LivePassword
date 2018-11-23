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

namespace LivePassword.Migration.Views
{
    public partial class WizardPage : PhoneApplicationPage
    {
        public WizardPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var p = await StorageHelper.LoadData("password.dat", typeof(LivePassword.classes.pPassword)) as LivePassword.classes.pPassword;
            if (p != null)
            {
                RemoveCredential();
                SaveCredential(p.password);
            }

            try
            {
                var check = await ApplicationData.Current.LocalFolder.GetFileAsync("data.sdf");
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("database.sqlite");
                var context = new DatabaseContext(DatabaseContext.ConnectionString);

                Sqlite.Database database;
                Sqlite.Statement statement;

                Sqlite.Sqlite3.sqlite3_open_v2(file.Path, out database, 2 | 4, string.Empty);

                var credit = context.CreditCardTable;
                foreach (var item in credit)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialCreditCard` (Title,Number,Expiration,CVV,PIN) VALUES('{0}','{1}','{2}','{3}','{4}');", Clear(item.title), Clear(item.number), Clear(item.scadenza), Clear(item.cvc), Clear(item.cvc)), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.number), "CredentialCreditCard", "creditcard"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }

                var login = context.LoginTable;
                foreach (var item in login)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialLogin` (Title,User,Password) VALUES('{0}','{1}','{2}');", Clear(item.title), Clear(item.user), Clear(item.password)), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.user), "CredentialLogin", "login"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }

                var mail = context.MailTable;
                foreach (var item in mail)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialMail` (Title,WebSite,Mail,Password) VALUES('{0}','{1}','{2}','{3}');", Clear(item.title), Clear(item.site), Clear(item.mail), Clear(item.password)), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.mail), "CredentialMail", "mail"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }

                var note = context.NoteTable;
                foreach (var item in note)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialNote` (Title,Note) VALUES('{0}','{1}','{2}','{3}');", Clear(item.title), Clear(item.text)), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.text), "CredentialNote", "note"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }

                var website = context.WebSiteTable;
                foreach (var item in website)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialWebSite` (Title,WebSite,User,Password) VALUES('{0}','{1}','{2}','{3}');", Clear(item.title), Clear(item.website), Clear(item.user), Clear(item.password)), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.user), "CredentialWebSite", "website"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }

                Sqlite.Sqlite3.sqlite3_close(database);

                context.Dispose();

                await check.DeleteAsync();
                StorageHelper.SafeDeleteFile(ApplicationData.Current.LocalFolder, "password.dat");
            }
            catch { }

            NavigationService.GoBack();
        }

        private string Clear(string str)
        {
            return str.Replace("'", "''");
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