using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword
{
    public class filedatamodel
    {
        private ObservableCollection<classes.FileList> _files = new ObservableCollection<classes.FileList>();
        public ObservableCollection<classes.FileList> files
        {
            get 
            {
                return _files;
            }
        }
        public filedatamodel()
        {
            _files.Add(new classes.FileList("White Collar 5X01", "abc", 5513135153, ".avi", "13 december 2014, 10:30", ".avi"));
            _files.Add(new classes.FileList("White Collar 5X07", "abc", 4866828, ".avi", "14 december 2014, 09:50", ".avi"));
            _files.Add(new classes.FileList("White Collar 5X05", "ccv", 863800, ".avi", "15 december 2014, 20:10", ".avi"));
            _files.Add(new classes.FileList("White Collar 5X02", "aac", 565662, ".avi", "16 december 2014, 12:15", ".avi"));
            _files.Add(new classes.FileList("White Collar 5X04", "bac", 2224684, ".avi", "17 december 2014, 11:45", ".avi"));
        }
    }
}
