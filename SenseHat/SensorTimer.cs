using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace SenseHat
{
    public class SensorTimer
    {
        private readonly HTS221 hts221 = new HTS221();
        private readonly LPS25H lps25h = new LPS25H();

        public delegate void SensorValueHandler(float value);

        public SensorValueHandler OnTemperature { get; set; }
        public SensorValueHandler OnHumidity { get; set; }

        public float Temp { get; set; }
        public float Humid { get; set; }

        private async Task Init()
        {
            await hts221.Init().ConfigureAwait(false);
            await lps25h.Init().ConfigureAwait(false);
        }

        //private void TimerElapsed(ThreadPoolTimer timer)
        //{
        //    hts221.Update();
        //    if (hts221.Temperature.HasValue)
        //        OnTemperature?.Invoke(hts221.Temperature.Value);
        //    if (hts221.Humidity.HasValue)
        //        OnHumidity?.Invoke(hts221.Humidity.Value);
        //}

        public SensorTimer(int period)
        {
            //Init().ContinueWith(t =>
            //{
            //    ThreadPoolTimer.CreatePeriodicTimer(TimerElapsed, TimeSpan.FromMilliseconds(period));
            //});

            Task.Run(async () =>
            {
                await Init().ConfigureAwait(false);
                while (true)
                {
                    hts221.Update();
                    if (hts221.Temperature.HasValue)
                        Temp = hts221.Temperature.Value;
                    if (hts221.Humidity.HasValue)
                        Humid = hts221.Humidity.Value;
                }
            });
        }

        //public SensorReading GetReading()
        //{
        //    return new SensorReading
        //    {
        //        Date = reading.Date,
        //        Temperature = reading.Temperature,
        //        Humidity = reading.Humidity,
        //        Pressure = reading.Pressure
        //    };
        //}

        //public SenseHatReader(int period)
        //{
        //    init().ContinueWith(t =>
        //    {
        //        while (true)
        //        {
        //            if (token.IsCancellationRequested)
        //                break;

        //            Task.Delay(2);


        //        }
        //    }, token);


        //    _task = Task.Factory.StartNew(async () =>
        //    {


        //        while (true)
        //        {
        //            if (token.IsCancellationRequested)
        //                break;

        //            await Task.Delay(2);

        //            _hts221.Update();
        //            Temperature = _hts221.Temperature;
        //            Humidity = _hts221.Humidity;
        //        }
        //        //}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //    }
        //}
    }
}
