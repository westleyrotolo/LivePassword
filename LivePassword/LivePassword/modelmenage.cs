using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword
{
    public class modelmenage
    {
        ObservableCollection<string> a = new ObservableCollection<string>();
        public ObservableCollection<string> keys
        {
            get
            {
                return a;
            }
        }
        public modelmenage()
        {
            a.Add("arrowgren@live.it");
            a.Add("westley@outlook.it");
            
    }
    }
}
