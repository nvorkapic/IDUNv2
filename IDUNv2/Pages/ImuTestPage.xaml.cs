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
        }

        private void Timer_Tick(object sender, object e)
        {
            var r = DAL.ImuSensorWatcher.Readings;
            viewModel.GetAcceleration(r);
            viewModel.GetGyroscope(r);
            viewModel.GetMagneticField(r);
            viewModel.GetFusionPose(r);

            #region FusionPose
            PitchAxis.Value = viewModel.PitchAxis;
            PitchAxisValue.Text = viewModel.PitchAxis.ToString("0.0000");
            RollAxis.Value = viewModel.RollAxis;
            RollAxisValue.Text = viewModel.RollAxis.ToString("0.0000");
            JawAxis.Value = viewModel.JawAxis;
            JawAxisValue.Text = viewModel.JawAxis.ToString("0.0000");
            #endregion

            #region Acceleration
            AccelerationX.Value = viewModel.Acceleration.X;
            AccelerationX.Maximum = viewModel.AccelerationMax.X;
            AccelerationX.Minimum = viewModel.AccelerationMin.X;
            AccelerationXValue.Text = viewModel.Acceleration.X.ToString("0.0000");

            AccelerationY.Value = viewModel.Acceleration.Y;
            AccelerationY.Minimum = viewModel.AccelerationMin.Y;
            AccelerationY.Maximum = viewModel.AccelerationMax.Y;
            AccelerationYValue.Text = viewModel.Acceleration.Y.ToString("0.0000");

            AccelerationZ.Value = viewModel.Acceleration.Z;
            AccelerationZ.Minimum = viewModel.AccelerationMin.Z;
            AccelerationZ.Maximum = viewModel.AccelerationMax.Z;
            AccelerationZValue.Text = viewModel.Acceleration.Z.ToString("0.0000");
     
            Accel.Value = viewModel.AcceloScalar;
            Accel.Minimum = viewModel.amin;
            Accel.Maximum = viewModel.amax;
            AccelValue.Text = viewModel.AcceloScalar.ToString("0.0000");
            #endregion

            #region MagneticField
            MagnetX.Value = viewModel.Magnet.X;
            MagnetXValue.Text = viewModel.Magnet.X.ToString("0.0000");
            MagnetY.Value = viewModel.Magnet.Y;
            MagnetYValue.Text = viewModel.Magnet.Y.ToString("0.0000");
            MagnetZ.Value = viewModel.Magnet.Z;
            MagnetZValue.Text = viewModel.Magnet.Z.ToString("0.0000");

            Magnet.Value = viewModel.MagnetScalar;
            Magnet.Minimum = viewModel.mmin;
            Magnet.Maximum = viewModel.mmax;
            MagnetValue.Text = viewModel.MagnetScalar.ToString("0.0000");
            #endregion

            #region Gyroscope
            GyroX.Value = viewModel.Gyro.X;
            GyroX.Minimum = viewModel.GyroMin.X;
            GyroX.Maximum = viewModel.GyroMax.X;
            GyroXValue.Text = viewModel.Gyro.X.ToString("0.0000");

            GyroY.Value = viewModel.Gyro.Y;
            GyroY.Minimum = viewModel.GyroMin.Y;
            GyroY.Maximum = viewModel.GyroMax.Y;
            GyroYValue.Text = viewModel.Gyro.Y.ToString("0.0000");

            GyroZ.Value = viewModel.Gyro.Z;
            GyroZ.Minimum = viewModel.GyroMin.Z;
            GyroZ.Maximum = viewModel.GyroMax.Z;
            GyroZValue.Text =viewModel.Gyro.Z.ToString("0.0000");

            Gyro.Value = viewModel.GyroScalar;
            Gyro.Minimum = viewModel.gmin;
            Gyro.Maximum = viewModel.gmax;
            GyroValue.Text = viewModel.GyroScalar.ToString("0.0000");
            #endregion
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
        }
    }
}
