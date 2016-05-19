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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerSettingsPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public ServerSettingsPage()
        {
            this.InitializeComponent();

            LoadData();
        }

        private void TBGotFocus(object sender, RoutedEventArgs e)
        {
            Confirmation.Visibility = Visibility.Collapsed;
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void PBGotFocus(object sender, RoutedEventArgs e)
        {

            osk.SetTarget(sender as PasswordBox);
            Confirmation.Visibility = Visibility.Collapsed;
            osk.Visibility = Visibility.Visible;
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            Confirmation.Visibility = Visibility.Collapsed;
            if (OIDTB.Text.Length == 0 || CURLTB.Text.Length == 0 || UNTB.Text.Length == 0 || PASSTB.Password.Length <= 3)
            {
                Warning.Visibility = Visibility.Visible;
            }
            else
            {
                if (Warning.Visibility == Visibility.Visible)
                    Warning.Visibility = Visibility.Collapsed;

                List<ServerViewModel> ServerData = new List<ServerViewModel>();

                ServerData.Add(new ServerViewModel { ObjectID = OIDTB.Text, URL= CURLTB.Text, Username = UNTB.Text, Password = PASSTB.Password });

                string json = JsonConvert.SerializeObject(ServerData.ToArray(), Formatting.Indented);
                StorageFile ConfigFile = await localFolder.CreateFileAsync("ServerData.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(ConfigFile, json);

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += Timer_Tick;
                timer.Start();
                Confirmation.Visibility = Visibility.Visible;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Confirmation.Visibility = Visibility.Collapsed;
            var timer = (DispatcherTimer)sender;
            timer.Stop();
        }
        public async void LoadData()
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("ServerData.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);

                List<ServerViewModel> ServerData = new List<ServerViewModel>();

                ServerData = JsonConvert.DeserializeObject<List<ServerViewModel>>(ConfigText);

                OIDTB.Text = ServerData.FirstOrDefault().ObjectID;
                CURLTB.Text = ServerData.FirstOrDefault().URL;
                UNTB.Text = ServerData.FirstOrDefault().Username;
                PASSTB.Password = ServerData.FirstOrDefault().Password;
            }
            catch
            {

            }        
        }

    }
}
