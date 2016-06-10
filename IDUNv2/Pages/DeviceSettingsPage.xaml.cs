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
    public sealed partial class DeviceSettingsPage : Page
    {
        private DeviceSettingsViewModel viewModel = new DeviceSettingsViewModel();

        public DeviceSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as PasswordBox);
            osk.Visibility = Visibility.Visible;
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (viewModel.ObjectID.Length == 0 ||
                viewModel.SystemID.Length == 0 ||
                viewModel.Username.Length == 0 ||
                viewModel.Password.Length <= 3 ||
                viewModel.URL.Length == 0)
            {
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Error, "Server Settings Error", "Please check that all information is entered correctly and try again!");
            }
            else
            {
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Server Settings Saved", "Server settings have been saved locally.");
            }
        }
    }
}
