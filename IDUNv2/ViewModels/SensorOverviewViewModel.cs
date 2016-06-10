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

        public CmdBarItem[] CmdBarItems { get; private set; }

        #endregion

        #region Bias

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

        #region CmdBar Actions

        private void ResetBias(string propName)
        {
            var pi = GetType().GetProperty(propName);
            pi.SetValue(this, 0.0f);
        }

        #endregion

        public SensorOverviewViewModel()
        {
            TemperatureSensor = DAL.GetSensor(SensorId.Temperature);
            HumiditySensor = DAL.GetSensor(SensorId.Humidity);
            PressureSensor = DAL.GetSensor(SensorId.Pressure);

            CmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Repair, "Bias", o => { return; })
            };
        }
    }
}
