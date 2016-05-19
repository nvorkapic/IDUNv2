using IDUNv2.ViewModels.Reports;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages.Reports
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Templates : Page
    {
        private TemplatesViewModel viewModel = new TemplatesViewModel();

        public Templates()
        {
            this.InitializeComponent();
            this.Loaded += Templates_Loaded;
        }

        private async void Templates_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.InitAsync();
            this.DataContext = viewModel;
            //viewModel.CurDiscovery = viewModel.DiscoveryList.FirstOrDefault();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //viewModel.
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CreateTemplate();
        }
    }
}
