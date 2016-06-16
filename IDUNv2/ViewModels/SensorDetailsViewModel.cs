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
        private Sensor _sensor;
        private List<SensorTrigger> _triggers;

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
            get { return Sensor != null ? DAL.GetSensorBias(Sensor.Id) : 0.0f; }
            set { if (Sensor != null) { DAL.SetSensorBias(Sensor.Id, value); Notify(); } }
        }

        public async Task InitAsync(Sensor sensor)
        {
            Sensor = sensor;
            Bias = DAL.GetSensorBias(sensor.Id);
            Triggers = await DAL.GetSensorTriggersFor(sensor.Id);
        }
    }
}
