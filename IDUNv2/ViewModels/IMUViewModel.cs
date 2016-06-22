using IDUNv2.DataAccess;
using IDUNv2.SensorLib.IMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class IMUViewModel
    {

        public Vector3 Acceleration;
        public Vector3 AccelerationMax = new Vector3(double.MinValue, double.MinValue, double.MinValue);
        public Vector3 AccelerationMin = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
        public double AcceloScalar;
        public double amax = double.MinValue;
        public double amin = double.MaxValue;

        public Vector3 Magnet;
        public double MagnetScalar;
        public double mmax;
        public double mmin;

        public Vector3 Gyro;
        public Vector3 GyroMax = new Vector3(double.MinValue, double.MinValue, double.MinValue);
        public Vector3 GyroMin = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
        public double GyroScalar;
        public double gmax = double.MinValue;
        public double gmin = double.MaxValue;

        public double PitchAxis;
        public double RollAxis;
        public double JawAxis;


        
        public void GetAcceleration(ImuSensorReadings r)
        {
            Acceleration = r.Acceleration;
            AccelerationMin = GetVectorMin(Acceleration, AccelerationMin);
            AccelerationMax = GetVectorMax(Acceleration, AccelerationMax);
            AcceloScalar = Math.Sqrt(Math.Pow(r.Acceleration.X, 2) + Math.Pow(r.Acceleration.Y, 2) + Math.Pow(r.Acceleration.Z, 2));
            amax = Math.Max(AcceloScalar, amax);
            amin = Math.Min(AcceloScalar, amin);
        }

        public void GetMagneticField(ImuSensorReadings r)
        {
            Magnet = r.MagneticField;

            MagnetScalar = Math.Sqrt(Math.Pow(r.MagneticField.X, 2) + Math.Pow(r.MagneticField.Y, 2) + Math.Pow(r.MagneticField.Z, 2));
            mmax = Math.Max(MagnetScalar, mmax);
            mmin = Math.Min(MagnetScalar, mmin);
        }

        public void GetGyroscope(ImuSensorReadings r)
        {
            Gyro = r.Gyro;
            GyroMax = GetVectorMax(GyroMax, Gyro);
            GyroMin = GetVectorMin(GyroMin, Gyro);
            GyroScalar = Math.Sqrt(Math.Pow(r.Gyro.AsDegrees.X, 2) + Math.Pow(r.Gyro.AsDegrees.Y, 2) + Math.Pow(r.Gyro.AsDegrees.Z, 2));
            gmax = Math.Max(GyroScalar, gmax);
            gmin = Math.Min(GyroScalar, gmin);
        }

        public Vector3 GetVectorMax(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 v = new Vector3(Math.Max(vectorA.X, vectorB.X), Math.Max(vectorA.Y, vectorB.Y), Math.Max(vectorA.Z, vectorB.Z));
            return v;
        }

        public Vector3 GetVectorMin(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 v = new Vector3(Math.Min(vectorA.X, vectorB.X), Math.Min(vectorA.Y, vectorB.Y), Math.Min(vectorA.Z, vectorB.Z));
            return v;
        }
        


        public void GetFusionPose(ImuSensorReadings r)
        {
            PitchAxis = r.FusionPose.AsDegrees.X;
            RollAxis = r.FusionPose.AsDegrees.Y;
            JawAxis = r.FusionPose.AsDegrees.Z;
        }
    }
}
