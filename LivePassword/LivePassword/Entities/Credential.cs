using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.Entities
{
    public class Credential : ICatalogItem
    {
        public string Id { get; set; }

        public int ItemId { get; set; }

        public int IdSummary { get; set; }
        public string Table { get; set; }

        public string SummaryKey { get; set; }
    }
}
