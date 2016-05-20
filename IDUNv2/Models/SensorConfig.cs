using System;
using System.Collections.Generic;
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

    public class SensorConfig
    {
        public bool Enabled { get; set; }
        public SensorType Type { get; set; }
        public List<ThresholdConfig> Thresholds { get; set; }
    }
}
