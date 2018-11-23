using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace LivePassword.Controls
{
    // DatePicker leggermente modificato per avere il PlaceholderText
    public class PlatformDatePicker : DatePicker
    {
        private const string ButtonPartName = "DateTimeButton";

        private ButtonBase _dateButtonPart;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dateButtonPart = (ButtonBase)GetTemplateChild(ButtonPartName);

            DeterminePlaceholderElementVisibility();
            ValueChanged += OnValueChanged;
           
        }

        private void OnValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {

            DeterminePlaceholderElementVisibility();

            if (ValidationString != null)
            {
                try
                {
                    var regex = new Regex(ValidationString);
                    var valid = regex.IsMatch(Value.Value.ToString());
                    VisualStateManager.GoToState(this, valid ? "Valid" : "NotValid", true);
                    IsValid = valid;
                }
                catch (Exception)
                {

                }

            }
        }

        private void DeterminePlaceholderElementVisibility()
        {
            if (Value.HasValue)
            {
                var binding = new Binding("ValueString");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                BindingOperations.SetBinding(_dateButtonPart, Button.ContentProperty, binding);

                var binding2 = new Binding("Foreground");
                binding2.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                BindingOperations.SetBinding(_dateButtonPart, Button.ForegroundProperty, binding2);
            }
            else
            {
                var binding = new Binding("PlaceholderText");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                BindingOperations.SetBinding(_dateButtonPart, Button.ContentProperty, binding);

                _dateButtonPart.Foreground = (SolidColorBrush)App.Current.Resources["PhoneDisabledBrush"];
            }
        }
        #region Header
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty =
                DependencyProperty.Register("HeaderText", typeof(string), typeof(PlatformDatePicker), new PropertyMetadata(string.Empty));

        #endregion


        #region ValidationText

        public string ValidationText
        {
            get { return (string)GetValue(ValidationTextProperty); }
            set { SetValue(ValidationTextProperty, value); }
        }

        public static readonly DependencyProperty ValidationTextProperty =
                DependencyProperty.Register("ValidationText", typeof(string), typeof(PlatformDatePicker), new PropertyMetadata(string.Empty));
        #endregion
        #region IsValid
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(PlatformDatePicker), new PropertyMetadata(new PropertyChangedCallback(OnIsValidChanged)));

        private static void OnIsValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as PlatformDatePicker;
            if (sender.IsValid != null)
            {
                VisualStateManager.GoToState(sender, (bool)e.NewValue ? "Valid" : "NotValid", true);
            }
           // var sender = d as PlatformDatePicker;
           // if (sender.IsValid != null)
           // {
           ////     VisualStateManager.GoToState(sender, (bool)e.NewValue ? "Valid" : "NotValid", true);
           //     if (!(bool)e.NewValue)
           //     {
           //         sender.BorderThickness = new Thickness(0, 0, 0, 3);
           //         sender.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0XE5, 0x14, 0x0));
                    
           //     }
           // }
        }
        #endregion
        #region PlaceholderText
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(PlatformDatePicker), new PropertyMetadata(null));
        #endregion          
        #region ValidationString
        public string ValidationString
        {
            get { return (string)GetValue(ValidationStringProperty); }
            set { SetValue(ValidationStringProperty, value); }
        }

        public static readonly DependencyProperty ValidationStringProperty =
            DependencyProperty.Register("ValidationString", typeof(string), typeof(DatePicker), new PropertyMetadata("^(?!\\s*$).+"));
        #endregion
    }
}
