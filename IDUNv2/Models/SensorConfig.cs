using IDUNv2.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public enum SensorType
    {
        Usage,
        Temperature,
        Pressure,
        Humidity,
        Accelerometer,
        Magnetometer,
        Gyroscope
    }

    public class SensorConfig : NotifyBase
    {
        private bool _enabled;

        public bool Enabled { get { return _enabled; } set { _enabled = value; Notify(); } }
        public SensorType Type { get; set; }
        public ObservableCollection<ThresholdConfig> Thresholds { get; set; }

        public SensorConfig(SensorType Type)
        {
            this.Type = Type;
            Thresholds = new ObservableCollection<ThresholdConfig>();
        }
    }
}
