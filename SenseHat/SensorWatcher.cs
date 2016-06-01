using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace SenseHat
{
    public class SensorWatcher
    {
        private readonly HTS221 hts221 = new HTS221();
        private readonly LPS25H lps25h = new LPS25H();

        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }

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
            Task.Run(async () =>
            {
                await Init().ConfigureAwait(false);
                while (true)
                {
                    hts221.Update();
                    lps25h.Update();

                    if (hts221.Temperature.HasValue)
                        Temperature = hts221.Temperature.Value;
                    if (hts221.Humidity.HasValue)
                        Humidity = hts221.Humidity.Value;
                    if (lps25h.Pressure.HasValue)
                        Pressure = lps25h.Pressure.Value;
                }
            });
        }
    }
}
