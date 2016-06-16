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
    public sealed partial class SensorDetailsPage : Page
    {
        #region Fields

        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private Random rnd = new Random();
        private SensorDetailsViewModel viewModel = new SensorDetailsViewModel();

        #endregion

        #region Constructors

        public SensorDetailsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        #endregion

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Start();

            var sensor = e.Parameter as Sensor;

            SG.SetRange(sensor.RangeMin, sensor.RangeMax);
            SG.SetDanger(sensor.DangerLo, sensor.DangerHi);

            switch (sensor.DeviceState)
            {
                case SensorDeviceState.Online:
                    SG.ColorDataLines = 0xFF00FF00;
                    break;
                case SensorDeviceState.Simulated:
                    SG.ColorDataLines = 0xFFFFFF00;
                    break;
                case SensorDeviceState.Offline:
                    SG.ColorDataLines = 0;
                    break;
            }

            var cmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Repair, "Repair", o => DAL.ClearSensorFaultState(sensor.Id)),
                new CmdBarItem(Symbol.Setting, "Settings", o => Frame.Navigate(typeof(SensorSettingsPage), sensor)),
            };

            DAL.PushNavLink(new NavLinkItem(sensor.Id.ToString(), GetType(), sensor));
            DAL.SetCmdBarItems(cmdBarItems);

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            await viewModel.InitAsync(sensor);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }

        #region Event Handlers

        private void CompositionTarget_Rendering(object sender, object e)
        {
            SG.Render();
        }

        private void Timer_Tick(object sender, object e)
        {
            float v = viewModel.Sensor.Value;
            SG.AddDataPoint(v);
        }

        private void ResetBias_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Bias = 0;
        }

        #endregion
    }
}
