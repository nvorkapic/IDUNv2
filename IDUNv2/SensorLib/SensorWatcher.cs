﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;

namespace IDUNv2.SensorLib
{
    /// <summary>
    /// Background thread which monitors sensors and checks triggers and danger zones
    /// </summary>
    public class SensorWatcher
    {
        #region Devices

        private static readonly HTS221 hts221 = new HTS221();
        private static readonly LPS25H lps25h = new LPS25H();

        #endregion

        private static readonly Random rnd = new Random();

        private ThreadPoolTimer pollTimer;

        public readonly Sensor[] Sensors = new Sensor[]
        {
            new Sensor(SensorId.Temperature, -40, 100, "° C"),
            new Sensor(SensorId.Humidity, 0, 100, "% RH"),
            new Sensor(SensorId.Pressure, 500, 2000, "hPa", "N0")
        };

        public SensorReadings Readings;
        public bool HasSensors { get; private set; }
        public float[] BiasValues { get; private set; }

        private async Task Init()
        {
            await hts221.Init();
            await lps25h.Init();

            if (hts221.IsValid)
            {
                Sensors[(int)SensorId.Temperature].DeviceState = SensorDeviceState.Online;
                Sensors[(int)SensorId.Temperature].HasHardware = true;
                Sensors[(int)SensorId.Humidity].DeviceState = SensorDeviceState.Online;
                Sensors[(int)SensorId.Humidity].HasHardware = true;
            }
            if (lps25h.IsValid)
            {
                Sensors[(int)SensorId.Pressure].DeviceState = SensorDeviceState.Online;
                Sensors[(int)SensorId.Pressure].HasHardware = true;
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

        /// <summary>
        /// Starts the background thread
        /// </summary>
        /// <param name="dispatcher">The CoreDispatcher for the UI thread</param>
        /// <param name="period">How often it should poll the sensors (in milliseconds)</param>
        public SensorWatcher(CoreDispatcher dispatcher, int period)
        {
            // setup simulated reading functions
            Sensors[0].GetSimValue = () => (float)(30.0 + rnd.NextDouble() * 5.0);
            Sensors[1].GetSimValue = () => (float)(30.0 + rnd.NextDouble() * 5.0);
            Sensors[2].GetSimValue = () => 1000.0f + (float)(50.0 - rnd.NextDouble() * 100.0);

            BiasValues = new float[Sensors.Length];

            Init().ContinueWith(task =>
            {
                pollTimer = ThreadPoolTimer.CreatePeriodicTimer(async timer =>
                {
                    if (hts221.IsValid)
                        hts221.GetReadings(ref Readings);
                    if (lps25h.IsValid)
                        lps25h.GetReadings(ref Readings);

                    var now = DateTime.Now;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Sensors[0].UpdateValue(now, Readings.Temperature, BiasValues[0]);
                        //Sensors[1].UpdateValue(now, Readings.Humidity, BiasValues[1]);
                        //Sensors[2].UpdateValue(now, Readings.Pressure, BiasValues[2]);
                    });
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //Sensors[0].UpdateValue(now, Readings.Temperature, BiasValues[0]);
                        Sensors[1].UpdateValue(now, Readings.Humidity, BiasValues[1]);
                        //Sensors[2].UpdateValue(now, Readings.Pressure, BiasValues[2]);
                    });
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //Sensors[0].UpdateValue(now, Readings.Temperature, BiasValues[0]);
                        //Sensors[1].UpdateValue(now, Readings.Humidity, BiasValues[1]);
                        Sensors[2].UpdateValue(now, Readings.Pressure, BiasValues[2]);
                    });

                }, TimeSpan.FromMilliseconds(period));
            });
        }
    }
}
