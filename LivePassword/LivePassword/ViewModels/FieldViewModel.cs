using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;

namespace LivePassword.ViewModels
{
    // Rappresenta ogni SINGOLO campo di ogni tipo di credenziale (Title, User, Pass ecc)
    // Le proprietà sono:
    // ColumnName: il nome della colonna, per le query
    // ColumnIndex: l'indice della colonna, per le query
    // InputScope: viene usato prima per scegliere l'ItemTemplate, e poi realmente per l'input scope
    //             ergo: Date mostra il DatePicker
    //                   AlphanumericFullWidth mostra una TextBox/TextBlock wrappabile
    //                   Gli altri per ora servono direttamente sulla TextBox 
    // Placeholder: è l'esempio
    // DisplayName: è la key della risorsa per la localizzazione
    // Value: è il value.
    public class FieldViewModel : INotifyPropertyChanged
    {
        public string DisplayName { get; set; }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public string ColumnName { get; set; }

        public int ColumnIndex { get; set; }

        public InputScopeNameValue InputScope { get; set; }

        public string Placeholder { get; set; }

        private bool _isValid;
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                if (value != _isValid)
                {
                    _isValid = value;
                    NotifyPropertyChanged("IsValid");
                }
            }
        }

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