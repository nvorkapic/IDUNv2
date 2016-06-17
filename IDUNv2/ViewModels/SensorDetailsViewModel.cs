using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SensorDetailsViewModel : NotifyBase
    {
        #region Fields

        private ISensorAccess sensorAccess;
        private ISensorTriggerAccess triggerAccess;

        #endregion

        #region Notify Fields

        private Sensor _sensor;
        private List<SensorTrigger> _triggers;

        #endregion

        #region Notify Properties

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public List<SensorTrigger> Triggers
        {
            get { return _triggers; }
            set { _triggers = value; Notify(); }
        }

        public float Bias
        {
            get { return Sensor != null ? sensorAccess.GetSensorBias(Sensor.Id) : 0.0f; }
            set { if (Sensor != null) { sensorAccess.SetSensorBias(Sensor.Id, value); Notify(); } }
        }

        #endregion

        public SensorDetailsViewModel(ISensorAccess sensorAccess, ISensorTriggerAccess triggerAccess)
        {
            this.sensorAccess = sensorAccess;
            this.triggerAccess = triggerAccess;
        }

        public async Task InitAsync(Sensor sensor)
        {
            Sensor = sensor;
            Bias = sensorAccess.GetSensorBias(sensor.Id);
            Triggers = await triggerAccess.GetSensorTriggersFor(sensor.Id);
        }
    }
}
