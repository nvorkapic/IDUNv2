using IDUNv2.DataAccess;
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
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        public ImuTestPage()
        {
            this.InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var r = DAL.ImuSensorWatcher.Readings;
            if (r.FusionPoseValid)
            {
                Pose.Text = r.FusionPose.ToString(true);
            }
            MagnetProgressBarX.Value =r.FusionPose.AsDegrees.X;
            MagnetProgressBarY.Value =r.FusionPose.AsDegrees.Y;
            MagnetProgressBarZ.Value =r.FusionPose.AsDegrees.Z;
            
            MFX.Text = r.FusionPose.AsDegrees.X.ToString();
            MFY.Text = r.FusionPose.AsDegrees.Y.ToString();
            MFZ.Text = r.FusionPose.AsDegrees.Z.ToString();
        }
    }
}
