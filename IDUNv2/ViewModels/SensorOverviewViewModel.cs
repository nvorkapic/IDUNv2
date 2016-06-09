﻿using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SensorOverviewViewModel : NotifyBase
    {
        public Sensor TemperatureSensor { get; private set; }
        public Sensor HumiditySensor { get; private set; }
        public Sensor PressureSensor { get; private set; }

        #region Bias Values

        private float _biasTemp;
        private float _biasHumid;
        private float _biasPress;

        public float BiasTemp
        {
            get { return _biasTemp; }
            set { _biasTemp = value; Notify(); }
        }
        public float BiasHumid
        {
            get { return _biasHumid; }
            set { _biasHumid = value; Notify(); }
        }
        public float BiasPress
        {
            get { return _biasPress; }
            set { _biasPress = value; Notify(); }
        }
        #endregion

        public ActionCommand<string> ResetCommand { get; private set; }

        private void ResetCommand_Execute(string propName)
        {
            var pi = GetType().GetProperty(propName);
            pi.SetValue(this, 0.0f);
        }

        public SensorOverviewViewModel()
        {
            ResetCommand = new ActionCommand<string>(ResetCommand_Execute);
            TemperatureSensor = DAL.SensorWatcher.GetSensor(SensorId.Temperature);
            HumiditySensor = DAL.SensorWatcher.GetSensor(SensorId.Humidity);
            PressureSensor = DAL.SensorWatcher.GetSensor(SensorId.Pressure);
        }
    }
}