using IDUNv2.Common;
using IDUNv2.DataAccess;
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

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public float Bias
        {
            get { return Sensor != null ? DAL.GetSensorBias(Sensor.Id) : 0.0f; }
            set { if (Sensor != null) { DAL.SetSensorBias(Sensor.Id, value); Notify(); } }
        }
    }
}
