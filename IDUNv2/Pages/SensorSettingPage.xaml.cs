
using System;
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
using Windows.Storage;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using IDUNv2.Models;
using IDUNv2.Services;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages
{

    public sealed partial class SensorSettingPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private static SQLiteConnection db = new SQLiteConnection(AppData.SqlitePlatform, AppData.DbPath);

        #region Keyboard
        private object _host;

        public Control TargetBox { get; private set; }

        public void SetTarget(TextBox control)
        {
            TargetBox = control;
        }

        public void RegisterTarget(TextBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }
        public void RegisterTarget(PasswordBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }

        public void RegisterHost(object host)
        {
            if (host != null)
            {
                _host = host;
            }
        }

        public object GetHost()
        {
            return _host;
        }


        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;
            if (t.FocusState == FocusState.Pointer)
            {
                //this.IsEnabled = true;
                //turn on the lights
            }
        }
        #endregion

        public SensorTriggerViewModel viewModel = new SensorTriggerViewModel();

        public SensorSettingPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            ElementCount();
            osk.SetTarget(ValueTB);
        }

        public float value = 0;
        private void AddTrigger(object sender, RoutedEventArgs e)
        {
 
            if (viewModel.SensorTriggerList.Where(x => x.SensorId == viewModel.CurrentTrigger.SensorId).
                Where(x => x.TemplateId == viewModel.CurrentTrigger.TemplateId).
                Where(x => x.Comparer == viewModel.CurrentTrigger.Comparer).
                Where(x => x.Value == viewModel.CurrentTrigger.Value).Count() == 0)
            {
                viewModel.AddTrigger();
                ElementCount();
            }
        
        }

        public void ElementCount()
        {
            TriggerCount.Text = viewModel.SensorTriggerList.Count().ToString();
            UsageNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Usage).Count().ToString();
            TemperatureNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Temperature).Count().ToString();
            PressureNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Pressure).Count().ToString();
            HumidityNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Humidity).Count().ToString();
            AccelerometerNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Accelerometer).Count().ToString();
            MagnetometerNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Magnetometer).Count().ToString();
            GyroscopeNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Gyroscope).Count().ToString();

        }

        private void ViewTriggerList(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TriggerList));
        }

        private void TBOnFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Visible;
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private void templateOnLoad(object sender, RoutedEventArgs e)
        {
            var lv = (ComboBox)sender;
            lv.SelectedItem = lv.Items.FirstOrDefault();
        }
    }
}

