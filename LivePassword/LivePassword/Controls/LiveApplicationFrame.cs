using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace LivePassword.Controls
{
    // Frame con l'header sempre visibile. 
    // Si modifica il titolino usando l'attached property .PageTitle (Bindabile)
    public class LiveApplicationFrame : TransitionFrame
    {
        public LiveApplicationFrame()
        {
            DefaultStyleKey = typeof(LiveApplicationFrame);

            Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var content = Content as UIElement;

            if (null == content)
                return;

            Title = GetPageTitle(content);
            //if (string.IsNullOrEmpty(Title))
            //{
            //    SystemTray.SetIsVisible(content, true);
            //    SystemTray.SetOpacity(content, 0);
            //}
        }

        #region PageTitle
        public static string GetPageTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(PageTitleProperty);
        }

        public static void SetPageTitle(DependencyObject obj, string value)
        {
            obj.SetValue(PageTitleProperty, value);
        }

        public static readonly DependencyProperty PageTitleProperty =
            DependencyProperty.RegisterAttached("PageTitle", typeof(string), typeof(LiveApplicationFrame), new PropertyMetadata(null, OnPageTitleChanged));

        private static void OnPageTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frame = App.RootFrame as LiveApplicationFrame;
            if (frame != null)
            {
                frame.Title = e.NewValue as string;
            }
        }
        #endregion

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LiveApplicationFrame), new PropertyMetadata(null));
        #endregion
    }
}
