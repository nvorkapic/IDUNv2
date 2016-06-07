using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
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
        public Sensor TemperatureSensor { get { return DAL.SensorWatcher.TemperatureSensor; } }
        public Sensor HumiditySensor { get { return DAL.SensorWatcher.HumiditySensor; } }
        public Sensor PressureSensor { get { return DAL.SensorWatcher.PressureSensor; } }

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
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        private Random rnd = new Random();
        private SensorPageViewModel viewModel = new SensorPageViewModel();

        public SensorPage()
        {
            this.InitializeComponent();

            viewModel.TemperatureSensor.RangeMin = -100;
            viewModel.TemperatureSensor.RangeMax = 100;
            viewModel.TemperatureSensor.DangerLo = -40;
            viewModel.TemperatureSensor.DangerHi = 80;

            viewModel.HumiditySensor.RangeMin = 0;
            viewModel.HumiditySensor.RangeMax = 100;
            viewModel.HumiditySensor.DangerLo = 10;
            viewModel.HumiditySensor.DangerHi = 95;

            viewModel.PressureSensor.RangeMin = 500;
            viewModel.PressureSensor.RangeMax = 2000;
            viewModel.PressureSensor.DangerLo = 800;
            viewModel.PressureSensor.DangerHi = 1800;

            this.DataContext = viewModel;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }

        private void Timer_Tick(object sender, object e)
        {
            SensorReadings readings = new SensorReadings();

            if (DAL.SensorWatcher.IsValid)
            {
                readings.Temperature = DAL.SensorWatcher.Readings.Temperature;
                readings.Humidity = DAL.SensorWatcher.Readings.Humidity;
                readings.Pressure = DAL.SensorWatcher.Readings.Pressure;
            }
            else
            {
                readings.Temperature = (float)(30.0 + rnd.NextDouble() * 2.1);
                readings.Humidity = (float)(30.0 + rnd.NextDouble() * 5.1);
                readings.Pressure = 1000.0f + (float)(50.0 - rnd.NextDouble() * 100.0);
            }

            readings.Temperature += viewModel.BiasTemp;
            readings.Humidity += viewModel.BiasHumid;
            readings.Pressure += viewModel.BiasPress;

            DAL.SensorWatcher.UpdateSensor(DAL.SensorWatcher.TemperatureSensor, readings);
            DAL.SensorWatcher.UpdateSensor(DAL.SensorWatcher.HumiditySensor, readings);
            DAL.SensorWatcher.UpdateSensor(DAL.SensorWatcher.PressureSensor, readings);
        }

        private void OnHumidity(float value)
        {
            temp = value;
        }

        private void OnTemperature(float value)
        {
            humid = value;
        }
    }
}
