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


    
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Notification> NotificationList { get; set; } = new ObservableCollection<Notification>();

       
        private Notification _latestNotification;
        public Notification LatestNotification { get { return _latestNotification; } set { _latestNotification = value; Notify(); } }

        public MainViewModel()
        {
            NotificationList.CollectionChanged += NotificationList_CollectionChanged;
        }

        private void NotificationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LatestNotification = NotificationList.FirstOrDefault();
        }



        public List<MainMenuItem> MainMenu { get { return NavigationService.MainMenu; } }

        private List<SubMenuItem> _subMenu;
        public List<SubMenuItem> SubMenu { get { return _subMenu; } set { _subMenu = value; Notify(); } }

        private MainMenuItem _curMainMenu;
        public MainMenuItem CurMainMenu { get { return _curMainMenu; } set { _curMainMenu = value; Notify(); } }
        private SubMenuItem _curSubMenu;
        public SubMenuItem CurSubMenu { get { return _curSubMenu; } set { _curSubMenu = value; Notify(); } }
        
        public void SelectMainMenu(Frame target, MainMenuItem item)
        {
            CurMainMenu = item;
            SubMenu = item.SubMenu;
            SelectSubMenu(target, SubMenu.First());
        }

        public void SelectSubMenu(Frame target, SubMenuItem item)
        {
            CurSubMenu = item;
            NavigationService.Navigate(target, item.PageType);
        }

        private WarningDialog _warningDialog;
        public WarningDialog WarningDialog { get {return _warningDialog;} set{_warningDialog = value; Notify(); } }

    }
}
