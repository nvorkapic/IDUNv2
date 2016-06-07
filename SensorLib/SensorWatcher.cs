using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace SensorLib
{
    public class SensorWatcher
    {
        public static Sensor UsageSensor = new Sensor("Usage", "GPIO8", "", "N0");
        public static Sensor TemperatureSensor = new Sensor("Temperature", "HTS221", "° C");
        public static Sensor HumiditySensor = new Sensor("Humidity", "HTS221", "% RH");
        public static Sensor PressureSensor = new Sensor("Pressure", "LPS25H", "hPa", "N0");

        private readonly HTS221 hts221 = new HTS221();
        private readonly LPS25H lps25h = new LPS25H();

        private ThreadPoolTimer timer;

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

        public SensorWatcher(int period)
        {
            Init().ContinueWith(task =>
            {
                timer = ThreadPoolTimer.CreatePeriodicTimer(t =>
                {
                    hts221.Update();
                    lps25h.Update();

                    if (hts221.Temperature.HasValue)
                        TemperatureSensor.UpdateValue(hts221.Temperature.Value);
                    if (hts221.Humidity.HasValue)
                        HumiditySensor.UpdateValue(hts221.Humidity.Value);
                    if (lps25h.Pressure.HasValue)
                        PressureSensor.UpdateValue(lps25h.Pressure.Value);

                }, TimeSpan.FromMilliseconds(period));
            });
            //Task.Run(async () =>
            //{
            //    await Init().ConfigureAwait(false);
            //    while (true)
            //    {
            //        await Task.Delay(2);

            //        hts221.Update();
            //        lps25h.Update();

            //        if (hts221.Temperature.HasValue)
            //            TemperatureSensor.UpdateValue(hts221.Temperature.Value);
            //        if (hts221.Humidity.HasValue)
            //            HumiditySensor.UpdateValue(hts221.Humidity.Value);
            //        if (lps25h.Pressure.HasValue)
            //            PressureSensor.UpdateValue(lps25h.Pressure.Value);
            //    }
            //});
        }
    }
}
