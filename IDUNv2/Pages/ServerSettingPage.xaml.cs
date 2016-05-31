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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerSettingPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public ServerSettingPage()
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
            if (OIDTB.Text.Length == 0 || CURLTB.Text.Length == 0 || UNTB.Text.Length == 0 || PASSTB.Password.Length <= 3 || SIDTB.Text.Length == 0)
            {
                //myTimer();
                //Warning.Visibility = Visibility.Visible;
                //Result.Foreground = new SolidColorBrush(Colors.Red);
                //Result.Text = "Please check that all information is entered correctly and try again!";
                //Tooltip.Visibility = Visibility.Visible;
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Error, "Server Settings Error", "Please check that all information is entered correctly and try again!");
                var dialog = new ContentDialog { Title = "Server Settings Error", Content = "Please check that all information is entered correctly and try again!", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RequestedTheme = ElementTheme.Dark, PrimaryButtonText = "OK" };

                var showdialog = await dialog.ShowAsync();
            }
            else
            {
                if (Warning.Visibility == Visibility.Visible)
                    Warning.Visibility = Visibility.Collapsed;

                List<ServerViewModel> ServerData = new List<ServerViewModel>();
                localSettings.Values["ObjectID"] = OIDTB.Text;
                localSettings.Values["SystemID"] = SIDTB.Text;
                localSettings.Values["URL"] = CURLTB.Text;
                localSettings.Values["Username"] = UNTB.Text;
                localSettings.Values["Password"] = PASSTB.Password;

                //myTimer();
                //Confirmation.Visibility = Visibility.Visible;
                //AppData.Notify.Add("NotifyString");
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Server Settings Saved", "Server settings have been saved locally.");
                //Result.Foreground = new SolidColorBrush(Colors.Green);
                //Result.Text = "Server settings successfully saved!";
                //Tooltip.Visibility = Visibility.Visible;
                var dialog = new ContentDialog { Title = "Server Settings Saved", Content = "Server settings have been saved locally.", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RequestedTheme = ElementTheme.Dark , PrimaryButtonText = "OK" };

                var showdialog = await dialog.ShowAsync();


                await AppData.InitCloud();
            }
        }

        //private void myTimer()
        //{
        //    DispatcherTimer timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromSeconds(3);
        //    timer.Tick += Timer_Tick;
        //    timer.Start();
        //}

        //private void Timer_Tick(object sender, object e)
        //{
        //    //Confirmation.Visibility = Visibility.Collapsed;
        //    Tooltip.Visibility = Visibility.Collapsed;
        //    var timer = (DispatcherTimer)sender;
        //    timer.Stop();
        //}
        public void LoadData()
        {
            try
            {

                OIDTB.Text =localSettings.Values["ObjectID"].ToString();
                SIDTB.Text = localSettings.Values["SystemID"].ToString();
                CURLTB.Text=localSettings.Values["URL"].ToString();
                UNTB.Text= localSettings.Values["Username"].ToString();
                PASSTB.Password=localSettings.Values["Password"].ToString();
            }
            catch
            {

            }        
        }

    }
}
