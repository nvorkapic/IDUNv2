using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.SensorLib;
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
    public class SensorSettingsViewModel : NotifyBase
    {
        private Sensor _sensor;

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public ActionCommand<object> SaveCommand { get; set; }

        public SensorSettingsViewModel()
        {
            SaveCommand = new ActionCommand<object>(SaveCommand_Execute);
        }

        private void SaveCommand_Execute(object param)
        {
            Sensor.SaveToLocalSettings();
        }
    }

    public sealed partial class SensorSettingsPage : Page
    {
        private SensorSettingsViewModel viewModel = new SensorSettingsViewModel();

        public SensorSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.Sensor = e.Parameter as Sensor;
            //viewModel.Sensor = DAL.SensorWatcher.TemperatureSensor;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(null as TextBox);
            osk.Visibility = Visibility.Collapsed;
        }
    }
}
