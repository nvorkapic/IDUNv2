using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.ViewModels
{
    public class SensorOverviewViewModel : NotifyBase
    {
        #region Fields

        private ISensorAccess sensorAccess;

        #endregion

        #region Properties

        public Sensor TemperatureSensor { get; private set; }
        public Sensor HumiditySensor { get; private set; }
        public Sensor PressureSensor { get; private set; }

        #endregion

        #region Bias

        public float BiasTemp
        {
            get { return sensorAccess.GetSensorBias(SensorId.Temperature); }
            set { sensorAccess.SetSensorBias(SensorId.Temperature, value); Notify(); }
        }
        public float BiasHumid
        {
            get { return sensorAccess.GetSensorBias(SensorId.Humidity); }
            set { sensorAccess.SetSensorBias(SensorId.Humidity, value); Notify(); }
        }
        public float BiasPress
        {
            get { return sensorAccess.GetSensorBias(SensorId.Pressure); }
            set { sensorAccess.SetSensorBias(SensorId.Pressure, value); Notify(); }
        }

        #endregion

        public SensorOverviewViewModel(ISensorAccess sensorAccess)
        {
            this.sensorAccess = sensorAccess;

            TemperatureSensor = sensorAccess.GetSensor(SensorId.Temperature);
            HumiditySensor = sensorAccess.GetSensor(SensorId.Humidity);
            PressureSensor = sensorAccess.GetSensor(SensorId.Pressure);
        }
    }
}
