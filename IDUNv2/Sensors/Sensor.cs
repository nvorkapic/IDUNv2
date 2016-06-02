using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Sensors
{
    public enum SensorId
    {
        Usage = 1,
        Temperature,
        Pressure,
        Humidity,
        Accelerometer,
        Magnetometer,
        Gyroscope
    }

    public enum SensorState
    {
        Normal,
        Faulted
    }

    public class Sensor
    {
        /// <summary>
        /// Size of databuffer. Must be a power of two.
        /// </summary>
        public const int BUFFER_SIZE = 128;

        public SensorId Id { get; set; }
        public string DeviceName { get; set; }
        public float RangeMin { get; set; }
        public float RangeMax { get; set; }
        public float DangerLo { get; private set; }
        public float DangerHi { get; private set; }
        public int? TemplateLoId { get; private set; }
        public int? TemplateHiId { get; private set; }
        public SensorState State { get; set; }

        private float[] dataBuffer = new float[BUFFER_SIZE];
        private int dataBufferIdx;

        public float Data { get; set; }

        public Sensor(SensorId id, string deviceName = "")
        {
            Id = id;
            DeviceName = deviceName;
        }

        public void UpdateData(float data)
        {
            Data = data;
            dataBuffer[dataBufferIdx] = data;
            dataBufferIdx = (dataBufferIdx + 1) & (BUFFER_SIZE - 1);
        }

        public void SetDangerLo(float val, int templateId)
        {
            DangerLo = val;
            TemplateLoId = templateId;
        }

        public void SetDangerHi(float val, int templateId)
        {
            DangerHi = val;
            TemplateHiId = templateId;
        }

        public static Sensor TemperatureSensor = new Sensor(SensorId.Temperature, "HTS221");
        public static Sensor HumiditySensor = new Sensor(SensorId.Humidity, "HTS221");
        public static Sensor PressureSensor = new Sensor(SensorId.Pressure, "LPS25H");
    }
}
