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

        public readonly Sensor TemperatureSensor = new Sensor(r => r.Temperature, "Temperature", "° C");
        public readonly Sensor HumiditySensor = new Sensor(r => r.Humidity, "Humidity", "% RH");
        public readonly Sensor PressureSensor = new Sensor(r => r.Pressure, "Pressure", "hPa", "N0");

        public SensorReadings Readings { get; } = new SensorReadings();
        public bool IsValid { get; private set; }

        private async Task Init()
        {
            await hts221.Init().ConfigureAwait(false);
            await lps25h.Init().ConfigureAwait(false);

            if (hts221.IsValid && lps25h.IsValid)
            {
                IsValid = true;
            }
        }

        public void UpdateSensor(Sensor s, SensorReadings readings = null)
        {
            if (readings == null)
                readings = Readings;

            s.UpdateValue(DateTime.Now, readings);
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
            //Task.Run(async () =>
            //{
            //    await Init().ConfigureAwait(false);
            //    while (true)
            //    {
            //        await Task.Delay(1);

            //        if (hts221.IsValid)
            //            hts221.Update(Readings);
            //        if (lps25h.IsValid)
            //            lps25h.Update(Readings);

            //        //if (hts221.Temperature.HasValue)
            //        //    TemperatureSensor.UpdateValue(hts221.Temperature.Value);
            //        //if (hts221.Humidity.HasValue)
            //        //    HumiditySensor.UpdateValue(hts221.Humidity.Value);
            //        //if (lps25h.Pressure.HasValue)
            //        //    PressureSensor.UpdateValue(lps25h.Pressure.Value);
            //    }
            //});
        }
    }
}
