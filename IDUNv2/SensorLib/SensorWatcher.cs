using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace IDUNv2.SensorLib
{
    public class SensorWatcher
    {
        private static readonly HTS221 hts221 = new HTS221();
        private static readonly LPS25H lps25h = new LPS25H();

        private ThreadPoolTimer pollTimer;

        public readonly Sensor[] Sensors = new Sensor[]
        {
            new Sensor(SensorId.Temperature, r => r.Temperature, -40, 100, "° C"),
            new Sensor(SensorId.Humidity, r => r.Humidity, 0, 100, "% RH"),
            new Sensor(SensorId.Pressure, r => r.Pressure, 500, 2000, "hPa", "N0")
        };

        //public readonly Sensor TemperatureSensor = new Sensor(r => r.Temperature, "Temperature", -40, 100, "° C");
        //public readonly Sensor HumiditySensor = new Sensor(r => r.Humidity, "Humidity", 0, 100, "% RH");
        //public readonly Sensor PressureSensor = new Sensor(r => r.Pressure, "Pressure", 500, 2000, "hPa", "N0");

        public SensorReadings Readings;
        public bool IsValid { get; private set; }

        private async Task Init()
        {
            await hts221.Init();
            await lps25h.Init();

            if (hts221.IsValid)
            {
                Sensors[(int)SensorId.Temperature].State = SensorState.Online;
                Sensors[(int)SensorId.Humidity].State = SensorState.Online;
            }
            if (lps25h.IsValid)
            {
                Sensors[(int)SensorId.Pressure].State = SensorState.Online;
            }

            IsValid = hts221.IsValid && lps25h.IsValid;
        }

        public Sensor GetSensor(SensorId id)
        {
            return Sensors[(int)id];
        }

        public void UpdateSensors(SensorReadings readings)
        {
            var now = DateTime.Now;
            foreach (var s in Sensors)
            {
                s.UpdateValue(now, readings);
            }
        }

        public void LoadSettings()
        {
            foreach (var s in Sensors)
            {
                s.LoadFromLocalSettings();
            }
        }

        public SensorWatcher(int period)
        {
            Init().ContinueWith(task =>
            {
                pollTimer = ThreadPoolTimer.CreatePeriodicTimer(timer =>
                {
                    if (hts221.IsValid)
                        hts221.GetReadings(ref Readings);
                    if (lps25h.IsValid)
                        lps25h.GetReadings(ref Readings);

                }, TimeSpan.FromMilliseconds(period));
            });
        }
    }
}
