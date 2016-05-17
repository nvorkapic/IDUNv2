
﻿using System;

﻿using IDUNv2.Pages.Settings.MeasurementConfig;
using IDUNv2.ViewModels;


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

namespace IDUNv2.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MeasurementsPage : Page
    {
        private MeasurementListSettingsVM viewModel = new MeasurementListSettingsVM(); 

        public MeasurementsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private void listOnLoad(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ListView)sender;
            selectedItem.SelectedIndex = 0;
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _listView = (ListView)sender;
            var _measurementItem = ((_listView.SelectedItem) as MeasurementListSettingsItems);

            viewModel.CurrentMeasurements = _measurementItem;

            ConfigurationFrame.Navigate(typeof(UsageConfig), _measurementItem);
   
            
        }
    }
}
