using IDUNv2.Models;
using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class ShellPage : Page
    {
        public static ShellPage Current;
        private ShellViewModel viewModel = new ShellViewModel();
        public Notification SelectedNotificationItem = new Notification();

        public ShellPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += MainPage_Loaded;
            Current = this;
            viewModel.NotificationList.CollectionChanged += NotificationList_CollectionChanged;
        }

        private void NotificationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (viewModel.NotificationList == null)
                NotificationButton.Visibility = Visibility.Collapsed;
            else
                NotificationButton.Visibility = Visibility.Visible;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var first = viewModel.NavList.First();
            viewModel.SelectMainMenu(ContentFrame, first);
        }

        private void NavMenuExpand_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsPaneOpen = !viewModel.IsPaneOpen;
        }

        private void NavMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lv = sender as ListView;
            var item = lv.SelectedItem as NavMenuItem;
            viewModel.SelectMainMenu(ContentFrame, item);
        }

        public void AddNotificatoin(NotificationType Type, string ShortDescription, string LongDescription)
        {
            DateTime Date = new DateTime();
            Date = DateTime.Now;
            viewModel.NotificationList.Insert(0, new Notification { Type = Type, ShortDescription = ShortDescription, LongDescription = LongDescription, Date = Date.ToString("dd/MM/yyyy HH:mm:ss") });
        }

        private void NotificationItemSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (NotificationFlyOutList.Items.Count != 0 && NotificationFlyOutList.SelectedItem != null)
            {
                var NotificationListView = (ListView)sender;
                SelectedNotificationItem = NotificationListView.SelectedItem as Notification;

                ExtendedNotification.DataContext = SelectedNotificationItem;
            }
        }

        private void NotificationViewed_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NotificationList.Remove(SelectedNotificationItem);
            if (NotificationFlyOutList.Items.Count == 0)
            {
                NotificationListPanel.Visibility = Visibility.Collapsed;
            }
            NotificationFlyOutList.SelectedItem = NotificationFlyOutList.Items.FirstOrDefault();
        }


        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotificationListPanel.Visibility == Visibility.Visible)
                NotificationListPanel.Visibility = Visibility.Collapsed;
            else
            {
                NotificationListPanel.Visibility = Visibility.Visible;
                NotificationFlyOutList.SelectedItem = NotificationFlyOutList.Items.FirstOrDefault();
            }
        }
    }
}
