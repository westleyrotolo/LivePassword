using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace LivePassword.Migration
{
    public static class MigrationWizard
    {
        public const string KEY = "wfm31228223929367";
        public static async Task<bool> Migrate()
        {
            var p = await StorageHelper.LoadData("password.dat", typeof(LivePassword.classes.pPassword)) as LivePassword.classes.pPassword;
            if (p != null)
            {
                RemoveCredential();
                SaveCredential(p.password);
            }

           try
            {    StorageHelper.SafeDeleteFile(ApplicationData.Current.LocalFolder, "password.dat");
         
                var check = await ApplicationData.Current.LocalFolder.GetFileAsync("data.sdf");
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("database.sqlite");
                var context = new DatabaseContext(DatabaseContext.ConnectionString);

                Sqlite.Database database;
                Sqlite.Statement statement;

                Sqlite.Sqlite3.sqlite3_open_v2(file.Path, out database, 2 | 4, string.Empty);

                var credit = context.CreditCardTable;
                foreach (var item in credit)
                {
                    var salt = Crypto.CreateSalt();
                    item.cvc = Convert.ToBase64String(Crypto.EncryptAes(item.cvc,KEY,salt));
                    item.number = Convert.ToBase64String(Crypto.EncryptAes(item.number,KEY,salt));
                    string pin = Convert.ToBase64String(Crypto.EncryptAes("0", KEY, salt));
                    string query = string.Format("INSERT INTO `CredentialCreditCard` (Title,Number,Expiration,CVV,PIN,Version,IdSync) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", Clear(item.title), Clear(item.number), Clear(item.scadenza), Clear(item.cvc),Clear(pin),DateTime.Now.Ticks,Guid.NewGuid().ToString());
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, query, out statement);
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
                    var salt = Crypto.CreateSalt();
                    item.password = Convert.ToBase64String(Crypto.EncryptAes(item.password, KEY, salt));
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialLogin` (Title,User,Password,Version,IdSync) VALUES('{0}','{1}','{2}','{3}','{4}');", Clear(item.title), Clear(item.user), Clear(item.password), DateTime.Now.Ticks, Guid.NewGuid().ToString()), out statement);
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
                    var salt = Crypto.CreateSalt();
                    item.password = Convert.ToBase64String(Crypto.EncryptAes(item.password, KEY, salt));
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialMail` (Title,WebSite,Mail,Password,Version,IdSync) VALUES('{0}','{1}','{2}','{3}','{4}','{5}');", Clear(item.title), Clear(item.site), Clear(item.mail), Clear(item.password), DateTime.Now.Ticks, Guid.NewGuid().ToString()), out statement);
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
                    var salt = Crypto.CreateSalt();
                    item.text = Convert.ToBase64String(Crypto.EncryptAes(item.text, KEY, salt));
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialNote` (Title,Note,Version,IdSync) VALUES('{0}','{1}','{2}','{3}');", Clear(item.title), Clear(item.text), DateTime.Now.Ticks, Guid.NewGuid().ToString()), out statement);
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
                    var salt = Crypto.CreateSalt();
                    item.password = Convert.ToBase64String(Crypto.EncryptAes(item.password, KEY, salt)); ;
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `CredentialWebSite` (Title,WebSite,User,Password,Version,IdSync) VALUES('{0}','{1}','{2}','{3}','{4}','{5}');", Clear(item.title), Clear(item.website), Clear(item.user), Clear(item.password), DateTime.Now.Ticks, Guid.NewGuid().ToString()), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);

                    var id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);

                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `DataCredentialSummary` (ItemId,Title,Subtitle,TableName,Category) VALUES({0},'{1}','{2}','{3}','{4}');", id, Clear(item.title), Clear(item.user), "CredentialWebSite", "website"), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                }
                var images = context.ImagesTable;
                foreach (var item in images)
                {
                    Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `FileCollection` (Title,TypeId) VALUES('{0}','images');", item.title), out statement);
                    Sqlite.Sqlite3.sqlite3_step(statement);
                    long Id = Sqlite.Sqlite3.sqlite3_last_insert_rowid(database);
                    Sqlite.Sqlite3.sqlite3_finalize(statement);
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    foreach (var img in item.Img)
                    {
                        StorageFile TempFile = await localFolder.GetFileAsync(img._name);
                        StorageFile _file = await TempFile.CopyAsync(localFolder, img._name, NameCollisionOption.GenerateUniqueName);
                        BasicProperties bp = await _file.GetBasicPropertiesAsync();
                        Sqlite.Sqlite3.sqlite3_prepare_v2(database, string.Format("INSERT INTO `FileItem` (CollectionId,DisplayName,Size,Extension,LocalPath,ItemDate) VALUES({0},'{1}',{2},'{3}','{4}','{5}');", Id, _file.DisplayName, bp.Size, _file.FileType, _file.Name, bp.ItemDate.Date.ToShortDateString() + ", " + bp.ItemDate.TimeOfDay.Hours.ToString("00") + ":" + bp.ItemDate.TimeOfDay.Minutes.ToString("00")), out statement);
                        Sqlite.Sqlite3.sqlite3_step(statement);
                        Sqlite.Sqlite3.sqlite3_finalize(statement);
                    }
                }
                
                Sqlite.Sqlite3.sqlite3_close(database);

                context.Dispose();

         await check.DeleteAsync();
         
                return true;
            }
            catch { }

          return false;
        }

        private static string Clear(string str)
        {
            return str.Replace("'", "''");
        }

        #region PasswordVault
        private static void SaveCredential(string password)
        {
            var vault = new PasswordVault();
            var credential = new PasswordCredential("Live Password", "Current User", password);

            vault.Add(credential);
        }

        private static string GetCredential()
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

        private static void RemoveCredential()
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
