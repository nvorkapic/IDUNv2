using IDUNv2.Common;
using IDUNv2.Models;
using SenseHat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public class SensorPageViewModel : NotifyBase
    {
        private float _temperature;
        private float _humidity;
        private float _pressure;

        public float Temperature
        {
            get { return _temperature; }
            set { _temperature = value; Notify(); }
        }

        public float Humidity
        {
            get { return _humidity; }
            set { _humidity = value; Notify(); }
        }

        public float Pressure
        {
            get { return _pressure; }
            set { _pressure = value; Notify(); }
        }
    }

    public sealed partial class SensorPage : Page
    {
        private float temp;
        private float humid;
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        private Random rnd = new Random();
        private SensorPageViewModel viewModel = new SensorPageViewModel();

        public SensorPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (AppData.SensorTimer.IsValid)
            {
                viewModel.Temperature = AppData.SensorTimer.Temperature;
                viewModel.Humidity = AppData.SensorTimer.Humidity;
                viewModel.Pressure = AppData.SensorTimer.Pressure;
            }
            else
            {
                viewModel.Temperature = (float)(30.0 + rnd.NextDouble() * 2.1);
                viewModel.Humidity = (float)(30.0 + rnd.NextDouble() * 5.1);
                viewModel.Pressure = (float)(30.0 + rnd.NextDouble() * 10.1);
            }
        }

        private void OnHumidity(float value)
        {
            temp = value;
        }

        private void OnTemperature(float value)
        {
            humid = value;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}
