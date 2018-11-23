using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LivePassword.Controls
{
    // TextBox con PlaceholderText e Password Mask (IsPassword = true)
    public class PlatformTextBox : TextBox, INotifyPropertyChanged
    {
        private ContentControl PlaceholderElement;
        private TextBlock ValidationTextBlock;
        public PlatformTextBox()
        {
            DefaultStyleKey = typeof(PlatformTextBox);
            TextChanged += OnTextChanged;
            this.TextInput += PlatformTextBox_TextInput;
            
         
        }

        void PlatformTextBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

                var regex = new Regex(ValidationString);
                var valid = regex.IsMatch(Text);

                VisualStateManager.GoToState(this, valid ? "Valid" : "NotValid", true);
                IsValid = valid;
           

        }

        public override void OnApplyTemplate()
        {
            ValidationTextBlock = (TextBlock)GetTemplateChild("TextValidation");
            PlaceholderElement = (ContentControl)GetTemplateChild("PlaceholderElement");
            DeterminePlaceholderElementVisibility();

            base.OnApplyTemplate();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (ValidationString != null)
            {
                var regex = new Regex(ValidationString);
                var valid = regex.IsMatch(Text);

                if (valid == false) return;
                VisualStateManager.GoToState(this, valid ? "Valid" : "NotValid", true);
                IsValid = valid;
            }
        }

        private void DeterminePlaceholderElementVisibility()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                PlaceholderElement.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderElement.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = this.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
                binding.UpdateSource();

            DeterminePlaceholderElementVisibility();

            if (ValidationString != null)
            {
                var regex = new Regex(ValidationString);
                var valid = regex.IsMatch(Text);
                if (valid == false) return;
                VisualStateManager.GoToState(this, valid ? "Valid" : "NotValid", true);
                IsValid = valid;
            }
        }

        #region PlaceholderText
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(PlatformTextBox), new PropertyMetadata(string.Empty));
        #endregion
        #region Header
        public string HeaderText
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register("HeaderText", typeof(string), typeof(PlatformTextBox), new PropertyMetadata(string.Empty));

        #endregion


        #region ValidationText

        public string ValidationText
        {
            get { return (string)GetValue(ValidationTextProperty); }
            set { SetValue(ValidationTextProperty, value); }
        }       
        
        public static readonly DependencyProperty ValidationTextProperty =
                DependencyProperty.Register("ValidationText", typeof(string), typeof(PlatformTextBox), new PropertyMetadata(string.Empty));
        #endregion

        #region IsPassword
        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        public static readonly DependencyProperty IsPasswordProperty =
            DependencyProperty.Register("IsPassword", typeof(bool), typeof(PlatformTextBox), new PropertyMetadata(false, OnIsPasswordChanged));

        private static void OnIsPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                (d as PlatformTextBox).FontFamily = new FontFamily("/Assets/Fonts/password.ttf#Password");
            }
            else
            {
                (d as PlatformTextBox).FontFamily = (FontFamily)App.Current.Resources["PhoneFontFamilyNormal"];
            }
        }
        #endregion

        #region IsValid
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value); NotifyPropertyChanged("IsValid");    
            }
        }
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool?), typeof(PlatformTextBox), new PropertyMetadata(new PropertyChangedCallback(OnIsValidChanged)));

    private static void OnIsValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
 	        var sender = d as PlatformTextBox;
            if (sender.IsValid != null)
            {
                       VisualStateManager.GoToState(sender, (bool)e.NewValue ? "Valid" : "NotValid", true);
            }
        }
        #endregion

        #region ValidationString
        public string ValidationString
        {
            get { return (string)GetValue(ValidationStringProperty); }
            set { SetValue(ValidationStringProperty, value); }
        }

        public static readonly DependencyProperty ValidationStringProperty =
            DependencyProperty.Register("ValidationString", typeof(string), typeof(PlatformTextBox), new PropertyMetadata("^(?!\\s*$).+"));
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
