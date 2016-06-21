using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.DataAccess;
using IDUNv2.ViewModels;
using System;
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
            ShellPage.Current.ShowWeb(new Uri ("ms-appx-web:///Assets/IDUNDocumentation.html"));
        }

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            ShellPage.Current.ShowWeb(new Uri ("ms-appx-web:///Assets/IDUNTutorial.html"));
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }
    }
}
