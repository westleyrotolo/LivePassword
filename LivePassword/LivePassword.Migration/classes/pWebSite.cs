﻿using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.classes
{
    [Table]
    public class pWebSite
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY")]
        private int fid { get; set; }

        [Column(DbType="NVarChar(100)")]
        public string id { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string title { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string user { get; set; }


        [Column(DbType = "NVarChar(100)")]
        public string password { get; set; }

        [Column(DbType = "NVarChar(100)")]
        public string website { get; set; }
    }
}
