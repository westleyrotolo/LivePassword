using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword
{
    public class DatabaseContext : DataContext
    {
        public const string ConnectionString = "Data Source=isostore:/data.sdf;Password='wfm31228232929367'";
        public DatabaseContext(string connectionString) : base(connectionString)
        {
        }
     
        public Table<classes.pLogin> LoginTable
        {
            get
            {
                return this.GetTable<classes.pLogin>();
            }
        }
        public Table<classes.pWebSite> WebSiteTable
        {
            get
            {
                return this.GetTable<classes.pWebSite>();
            }
        }
        public Table<classes.pMail> MailTable
        {
            get
            {
                return this.GetTable<classes.pMail>();
            }
        }
        public Table<classes.pNote> NoteTable
        {
            get
            {
                return this.GetTable<classes.pNote>();
            }
        }
        public Table<classes.pCreditCard> CreditCardTable
        {
            get
            {
                return this.GetTable<classes.pCreditCard>();
            }
        }
        public Table<classes.pImages> ImagesTable
        {
            get
            {
                return this.GetTable<classes.pImages>();
            }
        }
        public Table<classes.pGeneral> GeneralTable
        {
            get
            {
                return this.GetTable<classes.pGeneral>();
            }
        }
        public Table<classes.ImageList> imageList
        {
            get
            {
                return this.GetTable<classes.ImageList>();
            }
        }
        public Table<classes.pFiles> FileTable
        {
            get
            {
                return this.GetTable<classes.pFiles>();
            }
        }
        public Table<classes.FileList> fileList
        {
            get
            {
                return this.GetTable<classes.FileList>();
            }
        }
    }
}
