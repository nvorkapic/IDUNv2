using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class SensorOverviewPage : Page
    {
        #region Fields

        private Random rnd = new Random();
        private ISensorAccess sensorAccess = DAL.SensorAccess;
        private SensorOverviewViewModel viewModel;

        #endregion

        private Action<object> ShowDetailsFor(SensorId id)
        {
            return o =>
            {
                Frame.Navigate(typeof(Pages.SensorDetailsPage), sensorAccess.GetSensor(id));
            };
        }

        #region CmdBar Actions

        private void ResetBias(object param)
        {
            viewModel.BiasTemp = 0;
            viewModel.BiasHumid = 0;
            viewModel.BiasPress = 0;
        }

        #endregion

        #region Constructors

        public SensorOverviewPage()
        {
            viewModel = new SensorOverviewViewModel(sensorAccess);

            this.InitializeComponent();

            viewModel.TemperatureSensor.Command = new ActionCommand<object>(ShowDetailsFor(SensorId.Temperature));
            viewModel.HumiditySensor.Command = new ActionCommand<object>(ShowDetailsFor(SensorId.Humidity));
            viewModel.PressureSensor.Command = new ActionCommand<object>(ShowDetailsFor(SensorId.Pressure));

            this.DataContext = viewModel;
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(new CmdBarItem[]
                {
                    new CmdBarItem(Symbol.Play, "Activate All", o =>
                    {
                        viewModel.ActivateAll();
                    })
                });
        }

        #region Event Handlers

        private void ClearBias_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int i = Convert.ToInt32(btn.Tag);
            switch (i)
            {
                case 0:
                    viewModel.BiasTemp = 0;
                    break;
                case 1:
                    viewModel.BiasHumid = 0;
                    break;
                case 2:
                    viewModel.BiasPress = 0;
                    break;
            }
        }

        #endregion
    }
}
