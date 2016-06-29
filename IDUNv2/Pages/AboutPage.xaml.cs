using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.DataAccess;
using IDUNv2.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class AboutPage : Page
    {
        #region Constructors

        public AboutPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void Credentials_Click(object sender, RoutedEventArgs e)
        {
            Credentials.Visibility = Visibility.Visible;
        }

        private void Documentation_Click(object sender, RoutedEventArgs e)
        {
            ShellPage.Current.ShowWeb(new Uri("ms-appx-web:///Assets/IDUNDocumentation.html"));
        }

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            ShellPage.Current.ShowWeb(new Uri("ms-appx-web:///Assets/IDUNTutorial.html"));
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder TriggerReports = await localFolder.GetFolderAsync("TriggerReports");
            var file = await TriggerReports.GetFileAsync("TriggerReport");
            var data = await file.OpenReadAsync();
            using (var r = new StreamReader(data.AsStream()))
            {
                string text = r.ReadToEnd();
                var document = JsonConvert.DeserializeObject<DAL.DocumentString>(text);
                var details = document.Id + "\n\n" + document.Date + "\n\n" + document.DeviceID + "\n\n" + document.SystemID + "\n\n" + document.Value + " " + document.Unit + "\n\n" + document.DeviceState;

                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Report Loaded", details);
            }
        }
    }
}
