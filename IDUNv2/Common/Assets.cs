using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Common
{
    public static class Assets
    {
        private static Dictionary<SensorType, Tuple<string, string>> sensorIcons = new Dictionary<SensorType, Tuple<string, string>>()
        {
            { SensorType.Usage, new Tuple<string, string> ("/Assets/Finger.png", "" )},
            { SensorType.Temperature, new Tuple<string, string>("/Assets/Thermometer.png", "°C") },
            {SensorType.Pressure, new Tuple<string, string>("/Assets/Pressure.png", "hPa") },
            {SensorType.Humidity, new Tuple<string, string>("/Assets/Humidity.png", "%") },
            {SensorType.Accelerometer,new Tuple<string, string>("/Assets/Accelerometer.png", "m/s²") },
            {SensorType.Magnetometer, new Tuple<string, string>("/Assets/Magnet.png", "μT")},
            {SensorType.Gyroscope, new Tuple<string, string>("/Assets/Gyroscope.png", "rad/s") }

        };

        public static Dictionary<SensorType, Tuple<string, string>> SensorIcons { get { return sensorIcons; } }
    }
}
