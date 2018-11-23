using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.classes
{
    [Table]
    public class pCreditCard
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY")]
        private int fid { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string id { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string title { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string number { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string scadenza { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string cvc { get; set; }

        //[Column(DbType = "NVarChar(100)")]
        //public string code { get; set; }

        

    }

}
