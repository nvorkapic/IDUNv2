using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

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
    }
}
