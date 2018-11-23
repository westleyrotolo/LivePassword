using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Windows.UI.Core;

namespace LivePassword
{
    public static class PopupExtensions
    {
        public async static Task SlideToPage(this Popup self, SlideTransitionMode slidemode)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
self.IsOpen = true;
                var slide_transition = new SlideTransition { };
                slide_transition.Mode = slidemode;
                ITransition transition = slide_transition.GetTransition(self);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            });
            //CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{

            //  //  
            //});
        }

    }
    public static class UIElementExtensions
    {
        public async static Task SlideElement(this UIElement self, SlideTransitionMode slidemode)
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                self.Visibility = Visibility.Visible;
                var slide_transition = new SlideTransition { };
                slide_transition.Mode = slidemode;

                ITransition transition = slide_transition.GetTransition(self);

                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            });
        }
    }
}
