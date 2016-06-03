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
using IDUNv2.DataAccess;

namespace IDUNv2.Pages
{
    public sealed partial class ShellPage : Page
    {
        public static ShellPage Current;
        private ShellViewModel viewModel = new ShellViewModel();
        public Notification SelectedNotificationItem = new Notification();
        public static Grid Spinner;

        public ShellPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += MainPage_Loaded;
            Current = this;
            viewModel.NotificationList.CollectionChanged += NotificationList_CollectionChanged;

            Spinner = SpinnerPanel;
        }

        public static void SetSpinner(LoadingState state)
        {
            switch (state)
            {
                case LoadingState.Loading:
                    Spinner.Visibility = Visibility.Visible;
                    break;
                default:
                    Spinner.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void NotificationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(2500);
                timer.Tick += Timer_Tick;
                timer.Start();
                NotificationButton.Visibility = Visibility.Visible;

            }
                   
        }
        private void Timer_Tick(object sender, object e)
        {
            NotificationButton.Visibility = Visibility.Collapsed;
            var timer = (DispatcherTimer)sender;
            timer.Stop();
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
            else
            {
                ExtendedNotification.DataContext = new Notification { ShortDescription = "No Notifications", LongDescription = "Notifications have been read or the Notification List is Empty.", Type = NotificationType.Information, Date=DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
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

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (Image)sender;
            image.Source = new BitmapImage(new Uri(BaseUri, "Assets/loadinggif.gif"));
        }

        private void NotificationIconTap(object sender, RoutedEventArgs e)
        {
            NotificationButton.Visibility = Visibility.Collapsed;
            NotificationListPanel.Visibility = Visibility.Visible;
            NotificationFlyOutList.SelectedItem = NotificationFlyOutList.Items.FirstOrDefault();
        }

        private void CloseNotification_Click(object sender, RoutedEventArgs e)
        {
            NotificationListPanel.Visibility = Visibility.Collapsed;
        }

        private void NotificationFlyOutList_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (ListView)sender;
            list.SelectedItem = list.Items.FirstOrDefault();
        }
    }
}
