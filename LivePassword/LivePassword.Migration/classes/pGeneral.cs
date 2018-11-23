using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.classes
{
    [Table]
    public class pGeneral
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType="INT NOT NULL IDENTITY")]
        private int fid{get;set;}

        [Column(DbType="NVarChar(100)")]
        public string id { get; set; }

        [Column(DbType="NVarChar(100)")]
        public string title { get; set; }

        [Column(DbType="NVarChar(100)")]
        public string subtitle { get; set; }

        [Column(DbType="NVarChar(100)")]
        public string _type { get; set; }

        public pGeneral() { }

        public pGeneral(string Id, string Title, string Subtitle, string Type)
        {
            id = Id;
            title = Title;
            subtitle = Subtitle;
           
            _type = Type;
        }
    }
}
