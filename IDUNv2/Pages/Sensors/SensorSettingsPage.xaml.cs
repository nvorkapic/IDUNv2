using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class SensorSettingsPage : Page
    {
        private SensorSettingsViewModel viewModel = new SensorSettingsViewModel();

        public SensorSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            //this.Loaded += async (s, e) => await viewModel.InitAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel.Sensor = e.Parameter as Sensor;
            await viewModel.InitAsync();
            DAL.PushNavLink(new NavLinkItem("Settings", GetType(), e.Parameter));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        private void Templates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            var item = (FaultReportTemplate)cb.SelectedItem;
            if (item != null)
            {
                viewModel.SetSelectedTriggerFromTemplate(item);
            }
        }

        private void Triggers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.UpdateSelectedTemplate();
        }

        private void Pivot_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if (args.Item.Header.ToString() == "General")
                DAL.SetCmdBarItems(viewModel.GeneralCmdBarItems);
            else
            {
                DAL.SetCmdBarItems(viewModel.TriggerCmdBarItems);
            }
        }
    }
}
