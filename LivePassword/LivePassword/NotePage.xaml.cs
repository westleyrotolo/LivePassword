using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using LivePassword.Resources;

namespace LivePassword
{
    public partial class NotePage : PhoneApplicationPage
    {
        public NotePage()
        {
            InitializeComponent();
            txttitle.LostFocus += txttitle_LostFocus;
            txtnote.LostFocus += txttitle_LostFocus;
            txttitle.KeyUp += txttitle_KeyUp;
            txtnote.KeyUp += txtnote_KeyUp;
        }

        void txtnote_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                txtnote.Text += System.Environment.NewLine;
               
                txtnote.SelectionStart = txtnote.Text.ToString().Length;
                txtnote.SelectionLength = txtnote.Text.ToString().Length + 1;
            }
        }


        void txttitle_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            txtnote.Focus();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (App.mp == "M")
            //{
            //    App.mp = "";
            //    NavigationService.Navigate(new Uri("/MasterLogin.xaml", UriKind.Relative));
            //}
            txttitle.Focus();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.confirm;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.cancel;

            base.OnNavigatedTo(e);
        }
        void txttitle_LostFocus(object sender, RoutedEventArgs e)
        {
            RadTextBox t = (RadTextBox)sender;
            if (t.Text != "")
                t.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.NotValidated, "");
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((txttitle.Text == "") || (txtnote.Text == ""))
            {
                if (txttitle.Text == "")
                    txttitle.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty);
                if (txtnote.Text == "")
                    txtnote.ChangeValidationState(Telerik.Windows.Controls.PhoneTextBox.ValidationState.Invalid, AppResources.fieldnotempty     );
            }
            else
            {

                classes.pNote note = new classes.pNote();
                note.id = Guid.NewGuid().ToString();
                note.title = txttitle.Text;
                note.text = txtnote.Text;
                string t = "";
                t = note.text;
                t = t.Replace("\r", " ").Replace("\n"," ");
                t.ToString().Replace(System.Environment.NewLine, " ");
                if (t.Length >= 100)
                {      
                    t = t.ToString().Substring(0, 99) ;
                }
                classes.pGeneral g = new classes.pGeneral(note.id, note.title, t.ToString(), "Note");
                using (DatabaseContext db = new DatabaseContext(DatabaseContext.ConnectionString))
                {
                    db.NoteTable.InsertOnSubmit(note);
                    db.GeneralTable.InsertOnSubmit(g);
                    db.SubmitChanges();
                }
                NavigationService.GoBack();
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}