using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public delegate void SensorHandler(Sensor sender, float reading);

    public class Sensor
    {
        public string Name { get; set; }
        public List<SensorTrigger> Triggers { get; set; }

        public Sensor(string name)
        {
            Name = name;
        }
    }

    public static class SensorRegistry
    {
        private static Dictionary<Type, Sensor> sensors = new Dictionary<Type, Sensor>();

        public static void Add<T>(T sensor)
            where T : Sensor
        {
            sensors[typeof(T)] = sensor;
        }

        public static T Get<T>()
            where T : Sensor
        {
            return sensors[typeof(T)] as T;
        }

        public static void Foo()
        {
            SensorRegistry.Add(new Sensor("Temperature"));
        }
    }
}
