using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Resources;
using System.IO;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace LivePassword
{
    public partial class Traduzione : PhoneApplicationPage
    {
        public ObservableCollection<traslate> listWord = new ObservableCollection<traslate>();
        public Traduzione()
        {
            InitializeComponent();
          
            Load();
            localized.Text = System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName; 
            items.ItemsSource = listWord;
        }
        public void Load()
        {
            try
            {
                radbusy.Visibility = Visibility.Visible;
                radbusy.IsRunning = true;
            Uri uri = new Uri("Word.txt", UriKind.Relative);
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            using (StreamReader reader = new StreamReader(sri.Stream))
            {
                while(!reader.EndOfStream)
                    listWord.Add(new traslate(reader.ReadLine()));
            }
            } 
            catch (Exception ex)
            {
                
            }
            finally
            {
                radbusy.Visibility = Visibility.Collapsed;
                radbusy.IsRunning = false;
            }
           

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
         
           
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
        }

        private void txtcvv_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string r = (string)((RadTextBox)sender).Text;
            traslate t = (traslate)((RadTextBox)sender).DataContext;
            t.tradotta = r;
            listWord.Where(x => x.word == t.word).FirstOrDefault().tradotta = r;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask email = new Microsoft.Phone.Tasks.EmailComposeTask();
            string s = "";
            foreach (traslate t in listWord)
            {
                if (t.tradotta != "")
                {
                    s += t.word + ": " + t.tradotta + "\n";
                }
            }
            email.To = "arrowgreen@live.it";
            email.Body = s;
            email.Subject = "Translate Live Password | " + System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName;
            email.Show();
        }
    }
    public class traslate
    {
   
        public string word { get; set; }
        public string tradotta { get; set; }
        public string mark { get; set; }
        public traslate(string _word)
        {
            word = _word;
            tradotta = "";
            mark = "translate: " + _word;
        }
    }
}