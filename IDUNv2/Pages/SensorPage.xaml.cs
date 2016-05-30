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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages
{
    public sealed partial class SensorPage : Page
    {
        private float temp;
        private float humid;

        public SensorPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            temp = AppData.SensorTimer.Temp;

            humid = AppData.SensorTimer.Humid;
            TempText.Text = temp + " \u00B0C";
            HumidText.Text = humid + " \u00B0C";
        }

        private void OnHumidity(float value)
        {
            temp = value;
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => TempText.Text = value + " \u00B0C");
        }

        private void OnTemperature(float value)
        {
            humid = value;
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => HumidText.Text = value + " \u00B0C");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //var SubMenu = e.Parameter as List<SubMenuItem>;
            //AppData.SensorTimer.OnTemperature += OnTemperature;
            //AppData.SensorTimer.OnHumidity += OnHumidity;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //AppData.SensorTimer.OnTemperature -= OnTemperature;
            //AppData.SensorTimer.OnHumidity -= OnHumidity;
        }
    }
}
