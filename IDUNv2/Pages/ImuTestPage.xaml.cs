using IDUNv2.DataAccess;
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
    public sealed partial class ImuTestPage : Page
    {
        IMUViewModel viewModel = new IMUViewModel();

        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        public ImuTestPage()
        {
            this.InitializeComponent();
            timer.Tick += Timer_Tick;
            timer.Start();
            this.DataContext = viewModel;
        }

        private void Timer_Tick(object sender, object e)
        {
            var r = DAL.ImuSensorWatcher.Readings;
            viewModel.GetAcceleration(r);
            viewModel.GetMagneticField(r);
            viewModel.GetFusionPose(r);
            viewModel.GetGyroscope(r);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
        }
    }
}
