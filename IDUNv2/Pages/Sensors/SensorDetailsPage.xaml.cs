using IDUNv2.DataAccess;
using IDUNv2.SensorLib;
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
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private Random rnd = new Random();

        public SensorDetailsPage()
        {
            this.InitializeComponent();
            timer.Tick += Timer_Tick;
            timer.Start();

            SG.SetRange(-100, 100);
            SG.SetDanger(-40, 80);
        }

        private void Timer_Tick(object sender, object e)
        {
            var v = (rnd.NextDouble() * 20.0) - 10.0;
            SG.AddDataPoint((float)v);
        }
    }
}
