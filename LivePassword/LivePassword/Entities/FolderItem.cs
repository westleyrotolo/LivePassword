using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.Entities
{
    public class FolderItem
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public string DisplayName { get; set; }

        public string ItemDate { get; set; }
        public ulong Size { get; set; }

        public string Extension { get; set; }

        public string LocalPath { get; set; }

        public FolderItem() { }
        public FolderItem(string displayName, ulong size, string extension, string localPath,string itemDate)
        {
            DisplayName = displayName;
            Size = size;
            Extension = extension;
            LocalPath = localPath;
            ItemDate = itemDate;
        }
    }
}
