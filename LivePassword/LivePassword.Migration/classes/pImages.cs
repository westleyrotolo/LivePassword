using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.classes
{
    [Table]
    public class pImages
    {

        //[Column(Is, DbType = "INT NOT NULL IDENTITY")]
        //private int fid { get; set; }

        [Column(DbType = "NVarChar(100)", IsPrimaryKey=true )]
        public string id { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string title { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string number { get; set; }


        private EntitySet<ImageList> img = new EntitySet<ImageList>();
        [Association(Name = "ImageList", Storage = "img", OtherKey = "id", ThisKey = "id", DeleteRule = "CASCADE")]
        public EntitySet<ImageList> Img
        {
            get
            {
                return img;
            }
            set
            {
                img.Assign(value);
            }
        }
    }
        [Table]
        public class ImageList
        {
            private EntityRef<pImages> imageref = new EntityRef<pImages>();

            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY")]
            public int fid { get; set; }
            [Column(DbType = "NVarChar(100)")]
            public string id { get; set; }
            [Column]
            public string _name { get; set; }
            public ImageList() { }
            public ImageList(string s,string _id)
            {
                _name = s;
                id = _id;
            }            
            [Association(Name="ListImg", Storage = "imageref", ThisKey = "id", OtherKey = "id", IsForeignKey = true, DeleteRule="CASCADE")]
            public pImages _img
            {
                get
                {
                    return imageref.Entity;
                }
                set
                {
                    imageref.Entity = value;
                }
            }
        }
    
}