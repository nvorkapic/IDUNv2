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

        public readonly Sensor TemperatureSensor = new Sensor(r => r.Temperature, "Temperature", -40, 100, "° C");
        public readonly Sensor HumiditySensor = new Sensor(r => r.Humidity, "Humidity", 0, 100, "% RH");
        public readonly Sensor PressureSensor = new Sensor(r => r.Pressure, "Pressure", 500, 2000, "hPa", "N0");

        public SensorReadings Readings { get; } = new SensorReadings();
        public bool IsValid { get; private set; }

        private async Task Init()
        {
            await hts221.Init();
            await lps25h.Init();

            if (hts221.IsValid)
            {
                TemperatureSensor.State = SensorState.Online;
                HumiditySensor.State = SensorState.Online;
            }
            if (lps25h.IsValid)
            {
                PressureSensor.State = SensorState.Online;
            }

            IsValid = hts221.IsValid && lps25h.IsValid;
        }

        public void UpdateSensor(Sensor s, SensorReadings readings = null)
        {
            if (readings == null)
                readings = Readings;

            s.UpdateValue(DateTime.Now, readings);
        }

        public void LoadSettings()
        {
            TemperatureSensor.LoadFromLocalSettings();
            HumiditySensor.LoadFromLocalSettings();
            PressureSensor.LoadFromLocalSettings();
        }

        public SensorWatcher(int period)
        {
            Init().ContinueWith(task =>
            {
                pollTimer = ThreadPoolTimer.CreatePeriodicTimer(timer =>
                {
                    if (hts221.IsValid)
                        hts221.Update(Readings);
                    if (lps25h.IsValid)
                        lps25h.Update(Readings);

                }, TimeSpan.FromMilliseconds(period));
            });
        }
    }
}
