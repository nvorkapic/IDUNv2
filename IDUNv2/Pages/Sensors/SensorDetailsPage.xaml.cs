using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class SensorDetailsPage : Page
    {
        #region Fields

        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private Random rnd = new Random();
        private ISensorAccess sensorAccess = DAL.SensorAccess;
        private SensorDetailsViewModel viewModel;

        #endregion

        #region Constructors

        public SensorDetailsPage()
        {
            viewModel = new SensorDetailsViewModel(sensorAccess, DAL.SensorTriggerAccess);

            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        #endregion

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

        private void TriggerSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var trigger = (Sensor.Trigger)(sender as ListView).SelectedItem;
            SG.SetTrigger(trigger.val, trigger.cmp);
        }

        #endregion

        #region Navigation
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
                new CmdBarItem(Symbol.Repair, "Repair", o => sensorAccess.ClearSensorFaultState(sensor.Id)),
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
        #endregion
    }
}
