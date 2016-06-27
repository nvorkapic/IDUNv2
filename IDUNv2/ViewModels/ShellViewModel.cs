using IDUNv2.Common;
using IDUNv2.DataAccess;
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
    public class ShellViewModel : NotifyBase
    {
        #region Notify Fields

        private Notification _latestNotification;
        private Notification _selectedNotification;
        private string _notificationNumber;
        private Uri _localWebPage;
        private NavMenuItem _selectedNavMenuItem;
        private bool _isPaneOpen;

        #endregion

        #region Notify Properties

        public Notification LatestNotification
        {
            get { return _latestNotification; }
            set { _latestNotification = value; Notify(); }
        }

        public Notification SelectedNotificationItem
        {
            get { return _selectedNotification; }
            set { _selectedNotification = value; Notify(); }
        }

        public string NotificationNumber
        {
            get { return _notificationNumber; }
            set { _notificationNumber = value; Notify(); }
        }

        public Uri localWebPage
        {
            get { return _localWebPage; }
            set { _localWebPage = value; Notify(); }
        }

        public NavMenuItem SelectedNavMenuItem
        {
            get { return _selectedNavMenuItem; }
            set { _selectedNavMenuItem = value; Notify(); }
        }

        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { _isPaneOpen = value; Notify(); }
        }

        #endregion

        #region Properties

        public ObservableCollection<Notification> NotificationList { get; private set; }
        public ObservableCollection<NavLinkItem> NavLinks { get; private set; }

        private ObservableCollection<NavMenuItem> _navList;
        public ObservableCollection<NavMenuItem> NavList { get {return _navList; } private set { _navList = value; Notify(); } }

        #endregion

        private ObservableCollection<NavMenuItem> NavFullList = new ObservableCollection<NavMenuItem>()
        {
            new NavMenuItem { Label = "Sensors", Symbol = Symbol.View, PageType = typeof(Pages.SensorOverviewPage) },
            new NavMenuItem { Label = "Reports", Symbol = Symbol.ProtectedDocument, PageType = typeof(Pages.FaultReportListingPage) },
            new NavMenuItem { Label = "Templates", Symbol = Symbol.Document, PageType = typeof(Pages.FaultReportTemplateFormPage) },
            new NavMenuItem { Label = "Apps", Symbol = Symbol.AllApps, PageType = typeof(Pages.LEDControlPage) },
            new NavMenuItem { Label = "IMU", Symbol = Symbol.Bullets, PageType = typeof(Pages.ImuTestPage) },
            new NavMenuItem { Label = "Settings", Symbol = Symbol.Globe, PageType = typeof(Pages.DeviceSettingsPage) },
            new NavMenuItem { Label = "About", Symbol = Symbol.Help, PageType = typeof(Pages.AboutPage) }
        };

        private ObservableCollection<NavMenuItem> NoDeviceSettingsNavList = new ObservableCollection<NavMenuItem>()
        {
            new NavMenuItem { Label = "Settings", Symbol = Symbol.Globe, PageType = typeof(Pages.DeviceSettingsPage) }
        };

        public void SetNavListFull()
        {
            NavList = NavFullList;
            SelectedNavMenuItem = NavFullList.Where(x => x.Label == "Settings").FirstOrDefault();

        }
        public void SetNavListSettings()
        {
            NavList =NoDeviceSettingsNavList;
        }

        #region Constructors

        public ShellViewModel()
        {
            NotificationList = new ObservableCollection<Notification>();
            NavLinks = new ObservableCollection<NavLinkItem>();
            NotificationList.CollectionChanged += NotificationList_CollectionChanged;
        }

        #endregion

        #region Event Handlers

        private void NotificationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LatestNotification = NotificationList.FirstOrDefault();
            NotificationNumber = NotificationList.Count.ToString();
        }

        public void SelectMainMenu(Frame target, NavMenuItem item)
        {
            SelectedNavMenuItem = item;
            if (NavLinks.Count > 0)
                NavLinks.Clear();
            NavLinks.Add(new NavLinkItem(item.Label, item.PageType));
            target.Navigate(item.PageType);
        }

        #endregion
    }
}
