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
        #region Properties

        public Sensor TemperatureSensor { get; private set; }
        public Sensor HumiditySensor { get; private set; }
        public Sensor PressureSensor { get; private set; }

        #endregion

        #region Bias

        public float BiasTemp
        {
            get { return DAL.GetSensorBias(SensorId.Temperature); }
            set { DAL.SetSensorBias(SensorId.Temperature, value); Notify(); }
        }
        public float BiasHumid
        {
            get { return DAL.GetSensorBias(SensorId.Humidity); }
            set { DAL.SetSensorBias(SensorId.Humidity, value); Notify(); }
        }
        public float BiasPress
        {
            get { return DAL.GetSensorBias(SensorId.Pressure); }
            set { DAL.SetSensorBias(SensorId.Pressure, value); Notify(); }
        }

        #endregion

        public SensorOverviewViewModel()
        {
            TemperatureSensor = DAL.GetSensor(SensorId.Temperature);
            HumiditySensor = DAL.GetSensor(SensorId.Humidity);
            PressureSensor = DAL.GetSensor(SensorId.Pressure);
        }
    }
}
