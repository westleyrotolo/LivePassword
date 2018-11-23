using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.classes
{
    [Table]
    public class pFiles
    {
        [Column(DbType = "NVarChar(100)", IsPrimaryKey = true)]
        public string id { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string title { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string number { get; set; }

        private EntitySet<FileList> files = new EntitySet<FileList>();
        [Association(Name="FileList",Storage="files",OtherKey="id",DeleteRule="CASCADE")]
        public EntitySet<FileList> Files
        {
            get
            {
                return files;
            }
            set
            {
                files.Assign(value);
            }
        }
        
    }
    [Table]
    public class FileList
    {
        private EntityRef<pFiles> fileref = new EntityRef<pFiles>();

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY")]
        public int fid { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string id { get; set; }
        [Column]
        public string _name { get; set; }
        [Column]
        public ulong dimension { get; set; }
        [Column]
        public string thumbnail { get; set; }
        [Column]
        public string lastmodifica { get; set; }
        [Column]
        public string type { get; set; }
        public FileList() { }
        public FileList(string s, string _id, ulong _dimension, string _thumbnail, string _lastmodifica,string _type)
        {
            _name = s;
            id = _id;
            thumbnail = _thumbnail;
            dimension = _dimension;
            lastmodifica = _lastmodifica;
            type = _type;
        }
        [Association(Name="ListFiles",Storage="fileref", ThisKey="id", OtherKey="id",IsForeignKey=true)]
        public pFiles _files
        {
            get
            {
                return fileref.Entity;
            }
            set
            {
                fileref.Entity = value;
            }
        }
    }
}
