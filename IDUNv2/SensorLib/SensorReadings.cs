using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.SensorLib
{
    /// <summary>
    /// All readings from SensorWatcher
    /// </summary>
    public struct SensorReadings
    {
        public float? Temperature;
        public float? Humidity;
        public float? Pressure;
    }
}
