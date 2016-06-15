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
        private Random rnd = new Random();
        private SensorOverviewViewModel viewModel = new SensorOverviewViewModel();

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

        private void CompositionTarget_Rendering(object sender, object e)
        {
            if (viewModel.TemperatureSensor.Faulted)
                fire0.Draw();
            if (viewModel.HumiditySensor.Faulted)
                fire1.Draw();
            if (viewModel.PressureSensor.Faulted)
                fire2.Draw();
        }

        public SensorOverviewPage()
        {
            this.InitializeComponent();

            viewModel.TemperatureSensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Temperature)));
            viewModel.HumiditySensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Humidity)));
            viewModel.PressureSensor.Command = new ActionCommand<object>(ShowDetailsFor(DAL.GetSensor(SensorId.Pressure)));

            this.DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
    }
}
