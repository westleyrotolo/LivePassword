using LivePassword.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LivePassword.Common
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    // Sceglie il tipo di ItemTemplate a seconda dell'InputScope
    public class AddFieldTemplateSelector : DataTemplateSelector
    {
        // Normale TextBox
        public DataTemplate TextTemplate { get; set; }

        // TextBox wrappabile
        public DataTemplate FullWidthTemplate { get; set; }

        // DatePicker
        public DataTemplate DateTemplate { get; set; }
        public DataTemplate PasswordTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var field = item as FieldViewModel;
            if (field != null)
            {
                if (field.InputScope == System.Windows.Input.InputScopeNameValue.Date)
                {
                    return DateTemplate;
                }
                else if (field.InputScope == System.Windows.Input.InputScopeNameValue.AlphanumericFullWidth)
                {
                    return FullWidthTemplate;
                }
                else if (field.InputScope == System.Windows.Input.InputScopeNameValue.Password)
                {
                    return PasswordTemplate;
                }
                else if (field.InputScope == System.Windows.Input.InputScopeNameValue.EmailSmtpAddress)
                {
                    return UserTemplate;
                }

                return TextTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }

    // Uguale ma per il Get, visto che servono meno template
    public class GetFieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PasswordTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }

        public DataTemplate FullWidthTemplate { get; set; }
        public DataTemplate SiteTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var field = item as FieldViewModel;
            if (field != null)
            {
                if (field.InputScope == System.Windows.Input.InputScopeNameValue.AlphanumericFullWidth)
                {
                    return FullWidthTemplate;
                }
                if (field.InputScope == System.Windows.Input.InputScopeNameValue.Password)
                {
                    return PasswordTemplate;
                }
                if (field.InputScope == System.Windows.Input.InputScopeNameValue.Url)
                {
                    return SiteTemplate;
                }
                return TextTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
