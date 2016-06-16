using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
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
    public sealed partial class SensorOverviewPage : Page
    {
        #region Fields

        private Random rnd = new Random();
        private SensorOverviewViewModel viewModel = new SensorOverviewViewModel();

        #endregion

        private Action<object> ShowDetailsFor(Sensor s)
        {
            return o => Frame.Navigate(typeof(Pages.SensorDetailsPage), s);
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
            this.InitializeComponent();

            viewModel.TemperatureSensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Temperature)));
            viewModel.HumiditySensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Humidity)));
            viewModel.PressureSensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Pressure)));

            this.DataContext = viewModel;
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        #region Event Handlers

        private void CompositionTarget_Rendering(object sender, object e)
        {
            if (viewModel.TemperatureSensor.FaultState == SensorFaultState.Faulted)
                fire0.Draw();
            if (viewModel.HumiditySensor.FaultState == SensorFaultState.Faulted)
                fire1.Draw();
            if (viewModel.PressureSensor.FaultState == SensorFaultState.Faulted)
                fire2.Draw();
        }

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
