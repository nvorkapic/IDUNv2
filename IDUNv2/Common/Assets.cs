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
        private static Dictionary<SensorType, Tuple<string, string, string>> sensorIcons = new Dictionary<SensorType, Tuple<string, string, string>>()
        {
            {SensorType.Usage, new Tuple<string, string, string> ("/Assets/Finger.png", "", "Usage" )},
            {SensorType.Temperature, new Tuple<string, string, string>("/Assets/Thermometer.png", "°C","Temperature") },
            {SensorType.Pressure, new Tuple<string, string, string>("/Assets/Pressure.png", "hPa","Pressure") },
            {SensorType.Humidity, new Tuple<string, string, string>("/Assets/Humidity.png", "%", "Humidity") },
            {SensorType.Accelerometer,new Tuple<string, string, string>("/Assets/Accelerometer.png", "m/s²", "Accelerometer") },
            {SensorType.Magnetometer, new Tuple<string, string, string>("/Assets/Magnet.png", "μT", "Magnetometer")},
            {SensorType.Gyroscope, new Tuple<string, string, string>("/Assets/Gyroscope.png", "rad/s", "Gyroscope") }

        };

        public static Dictionary<SensorType, Tuple<string, string, string>> SensorIcons { get { return sensorIcons; } }
    }
}
