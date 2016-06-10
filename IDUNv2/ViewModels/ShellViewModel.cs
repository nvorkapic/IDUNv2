using IDUNv2.Common;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.ViewModels
{
    public class NavLinkItem
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
    }

    public class ShellViewModel : NotifyBase
    {
        public ObservableCollection<Notification> NotificationList { get; set; } = new ObservableCollection<Notification>();
        private Notification _latestNotification;
        public Notification LatestNotification { get { return _latestNotification; } set { _latestNotification = value; Notify(); } }

        public List<NavMenuItem> NavList { get; } = new List<NavMenuItem>()
        {
            new NavMenuItem { Label = "Sensor Overview", Symbol = Symbol.ViewAll, PageType = typeof(Pages.SensorOverviewPage) },
            new NavMenuItem { Label = "Fault Reports", Symbol = Symbol.ProtectedDocument, PageType = typeof(Pages.FaultReportListingPage) },
            new NavMenuItem { Label = "Templates", Symbol = Symbol.Document, PageType = typeof(Pages.FaultReportTemplateFormPage) },
            new NavMenuItem { Label = "Additional Apps", Symbol = Symbol.AllApps, PageType = typeof(Pages.LEDControlPage) },            
            new NavMenuItem { Label = "Device Settings", Symbol = Symbol.Globe, PageType = typeof(Pages.DeviceSettingsPage) },
            new NavMenuItem { Label = "About", Symbol = Symbol.Help, PageType = typeof(Pages.AboutPage) },
        };

        public ObservableCollection<NavLinkItem> NavLinks { get; set; } = new ObservableCollection<NavLinkItem>();

        public ShellViewModel()
        {
            NotificationList.CollectionChanged += NotificationList_CollectionChanged;
        }

        private void NotificationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LatestNotification = NotificationList.FirstOrDefault();
        }

        private NavMenuItem _selectedNavMenuItem;
        public NavMenuItem SelectedNavMenuItem
        {
            get { return _selectedNavMenuItem; }
            set { _selectedNavMenuItem = value; Notify(); }
        }

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { _isPaneOpen = value; Notify(); }
        }

        //private string _pageTitle;
        //public string PageTitle
        //{
        //    get { return _pageTitle; }
        //    set { _pageTitle = value; Notify(); }
        //}

        public void SelectMainMenu(Frame target, NavMenuItem item)
        {
            SelectedNavMenuItem = item;
            if (NavLinks.Count > 0)
                NavLinks.Clear();
            NavLinks.Add(new NavLinkItem { Title = item.Label, PageType = item.PageType });
            target.Navigate(item.PageType);
        }
    }
}
