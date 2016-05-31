using IDUNv2.Common;
using IDUNv2.Models;
using IDUNv2.Services;
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
        public ObservableCollection<Notification> NotificationList { get; set; } = new ObservableCollection<Notification>();
        private Notification _latestNotification;
        public Notification LatestNotification { get { return _latestNotification; } set { _latestNotification = value; Notify(); } }

        public List<NavMenuItem> NavList { get; } = new List<NavMenuItem>()
        {
            new NavMenuItem { Label = "Home", Symbol = Symbol.Home, PageType = typeof(Pages.MainPage) },
            new NavMenuItem { Label = "Measurements", Symbol = Symbol.Repair, PageType = typeof(Pages.SensorsPage) },
            new NavMenuItem { Label = "Reports", Symbol = Symbol.Document, PageType = typeof(Pages.ReportPage) },
            new NavMenuItem { Label = "LED Control", Symbol = Symbol.Pin, PageType = typeof(Pages.AdditionalApps.LEDControlPage) },
            new NavMenuItem { Label = "SpeechSynthesis", Symbol = Symbol.Microphone, PageType = typeof(Pages.AdditionalApps.SpeechSynthesisPage) },
            new NavMenuItem { Label = "Sensor Settings", Symbol = Symbol.Setting, PageType = typeof(Pages.SensorSettingPage) },
            new NavMenuItem { Label = "Device Settings", Symbol = Symbol.Globe, PageType = typeof(Pages.ServerSettingPage) },
            new NavMenuItem { Label = "About", Symbol = Symbol.Help, PageType = typeof(Pages.AboutPage) }
        };

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

        public void SelectMainMenu(Frame target, NavMenuItem item)
        {
            SelectedNavMenuItem = item;
            target.Navigate(item.PageType);
        }
    }
}
