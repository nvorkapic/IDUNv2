using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
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

namespace IDUNv2.Pages
{
    public class SensorDetailsViewModel : NotifyBase
    {
        private Sensor _sensor;

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public float Bias
        {
            get { return Sensor != null ? DAL.GetSensorBias(Sensor.Id) : 0.0f; }
            set { if (Sensor != null) { DAL.SetSensorBias(Sensor.Id, value); Notify(); } }
        }
    }

    public sealed partial class SensorDetailsPage : Page
    {
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private Random rnd = new Random();
        private SensorDetailsViewModel viewModel = new SensorDetailsViewModel();

        public SensorDetailsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Timer_Tick(object sender, object e)
        {
            float v = viewModel.Sensor.Value;
            SG.AddDataPoint(v);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Start();

            var sensor = e.Parameter as Sensor;

            SG.SetRange(sensor.RangeMin, sensor.RangeMax);
            SG.SetDanger(sensor.DangerLo, sensor.DangerHi);

            viewModel.Sensor = sensor;
            viewModel.Bias = DAL.GetSensorBias(sensor.Id);

            switch (sensor.State)
            {
                case SensorState.Online:
                    SG.ColorDataLines = 0xFF00FF00;
                    break;
                case SensorState.Simulated:
                    SG.ColorDataLines = 0xFFFFFF00;
                    break;
                case SensorState.Offline:
                    SG.ColorDataLines = 0;
                    break;
            }

            var cmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Clear, "Clear Bias", o => viewModel.Bias = 0),
                new CmdBarItem(Symbol.Repair, "Repair", o => DAL.ClearSensorFaultState(viewModel.Sensor.Id)),
                new CmdBarItem(Symbol.Setting, "Settings", o => Frame.Navigate(typeof(SensorSettingsPage), sensor)),
            };

            DAL.PushNavLink(new NavLinkItem(viewModel.Sensor.Id.ToString(), GetType(), e.Parameter));
            DAL.SetCmdBarItems(cmdBarItems);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            SG.Render();
        }
    }
}
