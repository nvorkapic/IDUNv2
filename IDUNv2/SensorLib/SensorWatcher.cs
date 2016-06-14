using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;

namespace IDUNv2.SensorLib
{
    public class SensorWatcher
    {
        private static readonly HTS221 hts221 = new HTS221();
        private static readonly LPS25H lps25h = new LPS25H();
        private static readonly Random rnd = new Random();

        private ThreadPoolTimer pollTimer;

        public readonly Sensor[] Sensors = new Sensor[]
        {
            new Sensor(SensorId.Temperature, -40, 100, "° C"),
            new Sensor(SensorId.Humidity, 0, 100, "% RH"),
            new Sensor(SensorId.Pressure, 500, 2000, "hPa", "N0")
        };

        public SensorReadings Readings;
        public SensorReadings BiasReadings;
        public bool HasSensors { get; private set; }

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

            HasSensors = hts221.IsValid && lps25h.IsValid;
        }

        public Sensor GetSensor(SensorId id)
        {
            return Sensors[(int)id];
        }

        public void LoadSettings()
        {
            foreach (var s in Sensors)
            {
                s.LoadFromLocalSettings();
            }
        }

        public SensorWatcher(CoreDispatcher dispatcher, int period)
        {
            BiasReadings.Temperature = 0;
            BiasReadings.Humidity = 0;
            BiasReadings.Pressure = 0;

            Init().ContinueWith(task =>
            {
                pollTimer = ThreadPoolTimer.CreatePeriodicTimer(async timer =>
                {
                    if (hts221.IsValid)
                        hts221.GetReadings(ref Readings);
                    if (lps25h.IsValid)
                        lps25h.GetReadings(ref Readings);

                    SensorReadings readings;

                    if (HasSensors)
                    {
                        readings = Readings;
                    }
                    else
                    {
                        readings.Temperature = (float)(30.0 + rnd.NextDouble() * 5.0);
                        readings.Humidity = (float)(30.0 + rnd.NextDouble() * 5.0);
                        readings.Pressure = 1000.0f + (float)(50.0 - rnd.NextDouble() * 100.0);
                    }

                    readings.Temperature += BiasReadings.Temperature;
                    readings.Humidity += BiasReadings.Humidity;
                    readings.Pressure += BiasReadings.Pressure;

                    var now = DateTime.Now;

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Sensors[0].UpdateValue(now, readings.Temperature);
                        Sensors[1].UpdateValue(now, readings.Humidity);
                        Sensors[2].UpdateValue(now, readings.Pressure);
                    });

                }, TimeSpan.FromMilliseconds(period));
            });
        }
    }
}
