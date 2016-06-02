using IDUNv2.Models;
using IDUNv2.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class ServerSettingPage : Page
    {
        //Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private DeviceSettingsViewModel viewModel = new DeviceSettingsViewModel();

        public ServerSettingPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private void TBGotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void PBGotFocus(object sender, RoutedEventArgs e)
        {

            osk.SetTarget(sender as PasswordBox);
            osk.Visibility = Visibility.Visible;
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (OIDTB.Text.Length == 0 || CURLTB.Text.Length == 0 || UNTB.Text.Length == 0 || PASSTB.Password.Length <= 3 || SIDTB.Text.Length == 0)
            {
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Error, "Server Settings Error", "Please check that all information is entered correctly and try again!");
                var dialog = new ContentDialog { Title = "Server Settings Error", Content = "Please check that all information is entered correctly and try again!", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RequestedTheme = ElementTheme.Dark, PrimaryButtonText = "OK" };
                var showdialog = await dialog.ShowAsync();
            }
            else
            {
                viewModel.ObjectID = OIDTB.Text;
                viewModel.SystemID = SIDTB.Text;
                viewModel.URL = CURLTB.Text;
                viewModel.Username = UNTB.Text;
                viewModel.Password = PASSTB.Password;
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Server Settings Saved", "Server settings have been saved locally.");
                var dialog = new ContentDialog { Title = "Server Settings Saved", Content = "Server settings have been saved locally.", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RequestedTheme = ElementTheme.Dark , PrimaryButtonText = "OK" };
                await AppData.InitCloud();
                var showdialog = await dialog.ShowAsync();
            }
        }
    }
}
