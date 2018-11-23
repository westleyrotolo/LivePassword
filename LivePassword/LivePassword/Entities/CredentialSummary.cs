using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword.Entities
{
    // Sommario della credenziale in MainPage
    public class CredentialSummary : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string title;
        public string subtitle;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string Subtitle
        {
            get
            {
                return subtitle;
            }
            set
            {
                subtitle = value;
                NotifyPropertyChanged("Subtitle"); ;
            }
        }

        public string Table { get; set; }

        public string Category { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
