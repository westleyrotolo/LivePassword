using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace LivePassword.Converters
{
    // InputScopeNameValue to InputScope
    public class InputScopeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();

            name.NameValue = (InputScopeNameValue)value;
            scope.Names.Add(name);

            return scope;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}