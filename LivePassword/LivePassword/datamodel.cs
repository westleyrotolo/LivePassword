using LivePassword.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePassword
{
    //public class datamodel
    //{
    //    public ObservableCollection<pGeneral> g = new ObservableCollection<pGeneral>();
    //    public List<Group<pGeneral>> general
    //    {
    //        get
    //        {         IEnumerable<classes.pGeneral> cityList = g;
    //                  return GetItemGroups(cityList, c => c._type);
    //        }
    //    }
    //    public datamodel()
    //    {

    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "Note"));
    //        g.Add(new pGeneral("abc", "Title", "Subtitle", "WebSite"));



    //    }
    //    public ObservableCollection<StringKeyGroup<classes.pGeneral>> GroupedItems(IEnumerable<classes.pGeneral> source)
    //    {
    //        return StringKeyGroup<classes.pGeneral>.CreateGroups(source,
    //            System.Threading.Thread.CurrentThread.CurrentUICulture,
    //            s => s._type, true);
    //    }

   
    //    private static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
    //    {
    //        IEnumerable<Group<T>> groupList = from item in itemList
    //                                          group item by getKeyFunc(item) into g
    //                                          orderby g.Key
    //                                          select new Group<T>(g.Key, g);

    //        return groupList.ToList();
    //    }

    //}
}
