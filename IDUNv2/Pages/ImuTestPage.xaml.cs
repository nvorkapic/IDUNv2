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
                PitchAxis.Value = r.FusionPose.AsDegrees.X;
                PitchAxisValue.Text = r.FusionPose.AsDegrees.X.ToString("0.0000") + " °";
                RollAxis.Value = r.FusionPose.AsDegrees.Y;
                RollAxisValue.Text = r.FusionPose.AsDegrees.Y.ToString("0.0000") + " °";
                JawAxis.Value = r.FusionPose.AsDegrees.Z;
                JawAxisValue.Text = r.FusionPose.AsDegrees.Z.ToString("0.0000") + " °";
            }

            if (r.AccelerationValid)
            {
                AccelerationX.Value = r.Acceleration.X;
                AccelerationXValue.Text = r.Acceleration.X.ToString("0.0000") + " g";
                AccelerationY.Value = r.Acceleration.X;
                AccelerationYValue.Text = r.Acceleration.Y.ToString("0.0000") + " g";
                AccelerationZ.Value = r.Acceleration.X;
                AccelerationZValue.Text = r.Acceleration.Z.ToString("0.0000") + " g";
            }

            if (r.MagneticFieldValid)
            {
                MagnetX.Value = r.MagneticField.X;
                MagnetXValue.Text = r.MagneticField.X.ToString("0.0000") + " uT";
                MagnetY.Value = r.MagneticField.Y;
                MagnetYValue.Text = r.MagneticField.Y.ToString("0.0000") + " uT";
                MagnetZ.Value = r.MagneticField.Z;
                MagnetZValue.Text = r.MagneticField.Z.ToString("0.0000") + " uT";
            }

            if (r.GyroValid)
            {
                GyroX.Value = r.Gyro.X;
                GyroXValue.Text = r.Gyro.AsDegrees.X.ToString("0.0000") + " °/s";
                GyroY.Value = r.Gyro.Y;
                GyroYValue.Text = r.Gyro.AsDegrees.Y.ToString("0.0000") + " °/s";
                GyroZ.Value = r.Gyro.X;
                GyroZValue.Text = r.Gyro.AsDegrees.Z.ToString("0.0000") + " °/s";
            }
           
        }
    }
}
