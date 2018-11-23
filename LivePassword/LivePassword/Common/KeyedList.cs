using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.Common
{
    // Per il LongListSelector (Anche se poi ho visto che già c'era una cosa simile, ma vabbè)
    public class KeyedList<TKey, TItem> : ObservableCollection<TItem>
    {
        public TKey Key { protected set; get; }

        public KeyedList(TKey key)
            : base()
        {
            Key = key;
        }

        public KeyedList(TKey key, IEnumerable<TItem> items)
            : base(items)
        {
            Key = key;
        }

        public KeyedList(IGrouping<TKey, TItem> grouping)
            : base(grouping)
        {
            Key = grouping.Key;
        }
    }
}
