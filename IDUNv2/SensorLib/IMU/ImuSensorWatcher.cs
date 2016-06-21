using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;

namespace IDUNv2.SensorLib.IMU
{
    public class ImuSensorWatcher
    {
        private readonly LSM9DS1ImuSensor imuSensor;
        private readonly CoreDispatcher coreDispatcher;
        private ThreadPoolTimer pollTimer;

        public ImuSensorReadings Readings { get; set; }

        public ImuSensorWatcher(CoreDispatcher coreDispatcher, int period)
        {
            this.coreDispatcher = coreDispatcher;

            imuSensor = new LSM9DS1ImuSensor(
                LSM9DS1Defines.ADDRESS0,
                LSM9DS1Defines.MAG_ADDRESS0,
                new LSM9DS1Config(),
                new SensorFusionRTQF());

            imuSensor.InitAsync().ContinueWith(task =>
            {
                pollTimer = ThreadPoolTimer.CreatePeriodicTimer(timer =>
                {
                    if (imuSensor.Initiated)
                    {
                        if (imuSensor.Update())
                        {
                            Readings = imuSensor.Readings;
                        }
                    }
                }, TimeSpan.FromMilliseconds(period));

            });
        }
    }
}
