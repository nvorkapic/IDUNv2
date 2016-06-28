using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.SensorLib.IMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class IMUViewModel : NotifyBase
    {
        private Vector3 acceleration;
        public Vector3 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; Notify(); }
        }
        private Vector3 accelerationMax;
        public Vector3 AccelerationMax
        {
            get { return accelerationMax; }
            set { accelerationMax = value; Notify(); }
        }
        private Vector3 accelerationMin;
        public Vector3 AccelerationMin
        {
            get { return accelerationMin; }
            set { accelerationMin = value; Notify(); }
        }
        private double acceloScalar;
        public double AcceloScalar
        {
            get { return acceloScalar; }
            set { acceloScalar = value; Notify(); }
        }
        private double _amax;
        public double amax
        {
            get { return _amax; }
            set { _amax = value; Notify(); }
        }
        private double _amin;
        public double amin
        {
            get { return _amin; }
            set { _amin = value; Notify(); }
        }
        private Vector3 magnet;
        public Vector3 Magnet
        {
            get { return magnet; }
            set { magnet = value; Notify(); }
        }
        private double magnetScalar;
        public double MagnetScalar
        {
            get { return magnetScalar; }
            set { magnetScalar = value; Notify(); }
        }
        private double _mmax;
        public double mmax
        {
            get { return _mmax; }
            set { _mmax = value; Notify(); }
        }
        private double _mmin;
        public double mmin
        {
            get { return _mmin; }
            set { _mmin = value; Notify(); }
        }

        private Vector3 gyro;
        public Vector3 Gyro
        {
            get { return gyro; }
            set { gyro = value; Notify(); }
        }
        private Vector3 gyroMax;
        public Vector3 GyroMax
        {
            get { return gyroMax; }
            set { gyroMax = value; Notify(); }
        }
        private Vector3 gyroMin;
        public Vector3 GyroMin
        {
            get { return gyroMin; }
            set { gyroMin = value; Notify(); }
        }
        private double gyroScalar;
        public double GyroScalar
        {
            get { return gyroScalar; }
            set { gyroScalar = value; Notify(); }
        }
        private double _gmax;
        public double gmax
        {
            get { return _gmax; }
            set { _gmax = value; Notify(); }
        }
        private double _gmin;
        public double gmin
        {
            get { return _gmin; }
            set { _gmin = value; Notify(); }
        }
        private double pitchAxis;
        public double PitchAxis
        {
            get { return pitchAxis; }
            set { pitchAxis = value; Notify(); }
        }
        private double rollAxis;
        public double RollAxis
        {
            get { return rollAxis; }
            set { rollAxis = value; Notify(); }
        }
        private double jawAxis;
        public double JawAxis
        {
            get { return jawAxis; }
            set { jawAxis = value; Notify(); }
        }

        public void GetAcceleration(ImuSensorReadings r)
        {
            Acceleration = r.Acceleration;
            AccelerationMin = GetVectorMin(Acceleration, AccelerationMin);
            AccelerationMax = GetVectorMax(Acceleration, AccelerationMax);
            AcceloScalar = Math.Sqrt(Math.Pow(r.Acceleration.X, 2) + Math.Pow(r.Acceleration.Y, 2) + Math.Pow(r.Acceleration.Z, 2));
            amax = Math.Max(AcceloScalar, amax) + 1;
            amin = Math.Min(AcceloScalar, amin) - 1;
        }

        public void GetMagneticField(ImuSensorReadings r)
        {
            Magnet = r.MagneticField;

            MagnetScalar = Math.Sqrt(Math.Pow(r.MagneticField.X, 2) + Math.Pow(r.MagneticField.Y, 2) + Math.Pow(r.MagneticField.Z, 2));
            mmax = Math.Max(MagnetScalar, mmax) + 1;
            mmin = Math.Min(MagnetScalar, mmin) - 1;
        }

        public void GetGyroscope(ImuSensorReadings r)
        {
            Gyro = r.Gyro.AsDegrees;
            GyroMax = GetVectorMax(GyroMax, Gyro);
            GyroMin = GetVectorMin(GyroMin, Gyro);
            GyroScalar = Math.Sqrt(Math.Pow(r.Gyro.AsDegrees.X, 2) + Math.Pow(r.Gyro.AsDegrees.Y, 2) + Math.Pow(r.Gyro.AsDegrees.Z, 2));
            gmax = Math.Max(GyroScalar, gmax) + 1;
            gmin = Math.Min(GyroScalar, gmin) - 1;
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
