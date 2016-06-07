using IDUNv2.Common;
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
    public class SensorPageViewModel : NotifyBase
    {
        public Sensor TemperatureSensor { get { return DAL.SensorWatcher.TemperatureSensor; } }
        public Sensor HumiditySensor { get { return DAL.SensorWatcher.HumiditySensor; } }
        public Sensor PressureSensor { get { return DAL.SensorWatcher.PressureSensor; } }

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

        public SensorPageViewModel()
        {
            ResetCommand = new ActionCommand<string>(ResetCommand_Execute);
        }
    }
}
