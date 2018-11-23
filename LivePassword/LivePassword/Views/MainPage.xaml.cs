using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LivePassword.ViewModels;
using LivePassword.Entities;
using LivePassword.Common;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using LivePassword.Resources;
using System.Globalization;

namespace LivePassword.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainViewModel ViewModel
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            if (e.NavigationMode == NavigationMode.New || App.MergeAvailable)
            {
                App.MergeAvailable = false;
                ViewModel.Initialize();
            }
 
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CatalogPage.xaml", UriKind.Relative));
        }

        private async void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as LongListSelector;
            var item = list.SelectedItem as CredentialSummary;

            if (item != null)
            {
                if (item.Table != "FileCollection")
                {
                    var args = new Credential
           {
               Id = item.Category,
               ItemId = item.ItemId,
               Table = item.Table,
               IdSummary = item.Id
           };

                    NavigationService.Navigate("/Views/CredentialGetPage.xaml", args);
                }
                else
                {
                 
                    var args = new Folder
                    {
                        Id = item.Category,
                        IdCollection = item.ItemId,
                        Title = item.Title,
                        IdSummary = item.Id,

                    };


                    NavigationService.Navigate("/Views/FolderGetPage.xaml", args);
                }

                list.SelectedItem = null;
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/about.xaml", UriKind.Relative));
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Traduzione.xaml", UriKind.Relative));

        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ChangePassword.xaml", UriKind.Relative));
        }
   
    }
    public class StringKeyGroup<T> : ObservableCollection<T>
    {
        public delegate string GetKeyDelegate(T item);
        public string Key { get; private set; }
        public StringKeyGroup(string key)
        {
            Key = key;
        }
        public static ObservableCollection<StringKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey, bool sort)
        {
            var list = new ObservableCollection<StringKeyGroup<T>>();

            foreach (var item in items)
            {
                var itemKey = getKey(item).Substring(0, 1).ToLower();

                var itemGroup = list.FirstOrDefault(li => li.Key == itemKey);
                var itemGroupIndex = itemGroup != null ? list.IndexOf(itemGroup) : -1;

                if (itemGroupIndex == -1)
                {
                    list.Add(new StringKeyGroup<T>(itemKey));
                    itemGroupIndex = list.Count - 1;
                }
                if (itemGroupIndex >= 0 && itemGroupIndex < list.Count)
                {
                    list[itemGroupIndex].Add(item);
                }
            }

            if (sort)
            {
                foreach (var group in list)
                {
                    group.ToList().Sort((c0, c1) => ci.CompareInfo.Compare(getKey(c0), getKey(c1)));
                }
            }

            return list;
        }



    }

    public class Group<T> : List<T>
    {
        public Group(string name, IEnumerable<T> items)
            : base(items)
        {
            this.Title = name;
        }

        public string Title
        {
            get;
            set;
        }
    }
}