using IDUNv2.ViewModels;
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
        private ReportsPageViewModel viewModel = new ReportsPageViewModel(AppData.Reports, AppData.FaultCodesCache);

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
            MainPage.Current.AddNotificatoin(Models.NotificationType.Information, "Template Saved", "Template is configured and saved and ready for use!");
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CreateTemplate();
            MainPage.Current.AddNotificatoin(Models.NotificationType.Information, "Template Created", "New Template has been added. Please configure and save to ensure proper functionality!");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            int dirtyTemplatesNr = 0;
            foreach (var item in viewModel.Templates)
            {
                if (item.Dirty)
                    ++dirtyTemplatesNr;
            }
            if (dirtyTemplatesNr >= 1)
            {
                MainPage.Current.AddNotificatoin(Models.NotificationType.Error, "Unsaved Templates", "Changed templates data and/or new templates were present and not saved. All changes and newly generated templates were discarded! ");
            }


            }

        }
}
