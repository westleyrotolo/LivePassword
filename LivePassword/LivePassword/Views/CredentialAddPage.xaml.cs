using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LivePassword.ViewModels;
using LinqToVisualTree;
using LivePassword.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace LivePassword.Views
{
    public partial class CredentialAddPage : PhoneApplicationPage
    {
        public CredentialAddPage()
        {
            InitializeComponent();

            // Non mi piace il fatto che vengano coperti gli elementi dalla keyboard, meglio lasciare il focus all'utente
            Loaded += OnLoaded;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //var textBox = this.Descendants<PlatformTextBox>().FirstOrDefault() as PlatformTextBox;

            //if (textBox != null)
            //{
            //    textBox.Focus(); 
            //    var item = textBox.DataContext as FieldViewModel;
            //    textBox.LostFocus += textBox_LostFocus;
            //}
        }
        string title = "";
        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
        
        }
        public bool websiteormail = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                (DataContext as CredentialViewModel).InitializeAdd(NavigationContext.QueryString["args"]);
                if ((NavigationContext.QueryString["args"].ToString().Contains("CredentialMail")))
                    websiteormail=true;
                else if ((NavigationContext.QueryString["args"].ToString().Contains("CredentialWebSite")))
                    websiteormail=true;
            }
        }

    
        private void Field_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == System.Windows.Input.Key.Enter)
            //{

            //    var text = sender as TextBox;
            //    var item = text.DataContext as FieldViewModel;
            //    var textBox = this.Descendants<TextBox>().SkipWhile(x=>x.GetHashCode()!=text.GetHashCode()).Skip(1).FirstOrDefault() as TextBox;
            //    if (textBox != null)
            //    {
            //        if (textBox.Text == "")
            //        {
            //            var t = this.Descendants<TextBox>().FirstOrDefault() as TextBox;
            //            var item2 = textBox.DataContext as FieldViewModel;
            //            if (t != null)
            //            {
            //                if (item2.DisplayName.ToLower().Contains("web"))
            //                    if ((t.Text != "") && (t.Text.IndexOf(" ")==-1))
            //                    {
            //                        textBox.Text = "https://" + t.Text.ToLower() + ".com";
            //                        textBox.SelectAll();
            //                    }
            //            }
            //        }
            //        textBox.Focus();

            //    }
            //    else
            //    {
            //        var textBox2 = this.Descendants<TextBox>().FirstOrDefault() as TextBox;
            //        if (textBox2 != null)
            //        {
            //            textBox2.Focus();
            //        }
            //    }

            //}
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        //private void Field_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    var text = sender as TextBox;
        //    var item = text.DataContext as FieldViewModel;
        //    if (!(string.IsNullOrEmpty(text.Text)))
        //        item.error = false;
        //}
        private ObservableCollection<string> users = new ObservableCollection<string>();
   

        public void Create()
        {
            Sqlite.Database database;
            Sqlite.Statement statement2;
            // Carica tutti i field disponibili per questo tipo di credenziale (Magari la query sarebbe da sostituire con uno string.format
            // E ancora meglio si potrebbe fare una classe con tutte le query CONST
            Sqlite.Sqlite3.sqlite3_open_v2(App.DatabasePath, out database, 2 | 4, string.Empty);

            users.Clear();
            Sqlite.Sqlite3.sqlite3_prepare_v2(database, "SELECT User FROM CredentialLogin UNION Select Mail AS User From CredentialMail UNION Select User From CredentialWebSite", out statement2);
            while (Sqlite.Sqlite3.sqlite3_step(statement2) == 100)
            {
                if (!(users.Contains(Sqlite.Sqlite3.sqlite3_column_text(statement2, 0))))
                    users.Add(Sqlite.Sqlite3.sqlite3_column_text(statement2, 0));
            }
            Sqlite.Sqlite3.sqlite3_finalize(statement2);
            Sqlite.Sqlite3.sqlite3_close(database);
        }
        private void AutoCompleteBox_Loaded(object sender, RoutedEventArgs e)
        {
            Create();
            AutoCompleteBox autocompletebox = (AutoCompleteBox)sender;
             autocompletebox.ItemsSource = users;
        }
        private void PlatformDatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            //if (b) { b = false; return; }
            //b = true;
            //var text = sender as AutoCompleteBox;
            //if (text == null) return;
            //var item = text.DataContext as FieldViewModel;
            //var textBox = this.Descendants<TextBox>().SkipWhile(x => x.GetHashCode() != text.GetHashCode()).Skip(1).FirstOrDefault() as TextBox;
            //if (textBox!=null)
            //textBox.Focus();     
        }

        private void PlatformTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "")
            {
                var t = this.Descendants<TextBox>().FirstOrDefault() as TextBox;
                var item2 = textBox.DataContext as FieldViewModel;
                if (t != null)
                {
                    if (item2.DisplayName.ToLower().Contains("web"))
                        if ((t.Text != "") && (t.Text.IndexOf(" ") == -1))
                        {
                            textBox.Text = "https://" + t.Text.ToLower() + ".com";
                            textBox.SelectAll();
                        }
                }
            }
        }



    }
}