using IDUNv2.Common;
using IDUNv2.Models;
using SenseHat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

        #region Bias Values

        private float _biasTemp;
        private float _biasHumid;
        private float _biasPress;

        public float BiasTemp
        {
            get { return _biasTemp; }
            set { _biasTemp = value; Notify(); }
        }
        public float BiasHumid
        {
            get { return _biasHumid; }
            set { _biasHumid = value; Notify(); }
        }
        public float BiasPress
        {
            get { return _biasPress; }
            set { _biasPress = value; Notify(); }
        }
        #endregion

        public ActionCommand<string> ResetCommand { get; private set; }

        private void ResetCommand_Execute(string propName)
        {
            var pi = GetType().GetProperty(propName);
            pi.SetValue(this, 0.0f);
        }

        public SensorPageViewModel()
        {
            ResetCommand = new ActionCommand<string>(ResetCommand_Execute);
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
            if (AppData.SensorWatcher.IsValid)
            {
                viewModel.Temperature = AppData.SensorWatcher.Temperature;
                viewModel.Humidity = AppData.SensorWatcher.Humidity;
                viewModel.Pressure = AppData.SensorWatcher.Pressure;
            }
            else
            {
                viewModel.Temperature = (float)(30.0 + rnd.NextDouble() * 2.1);
                viewModel.Humidity = (float)(30.0 + rnd.NextDouble() * 5.1);
                viewModel.Pressure = 1000.0f + (float)(50.0 - rnd.NextDouble() * 100.0);
            }

            viewModel.Temperature += viewModel.BiasTemp;
            viewModel.Humidity += viewModel.BiasHumid;
            viewModel.Pressure += viewModel.BiasPress;
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
