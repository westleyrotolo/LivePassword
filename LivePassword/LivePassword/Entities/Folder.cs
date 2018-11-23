using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.Entities
{
    public interface ICatalogItem
    {
        string Id { get; set; }
    }

    public class Folder : ICatalogItem
    {
        public int IdCollection { get; set; }
        public string Id { get; set; }
        public int IdSummary { get; set; }

        public string SupportedFiles { get; set; }
        public string Title { get; set; }
    }
}
