using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.SensorLib;

namespace IDUNv2.DataAccess
{
    public class SensorAccess : ISensorAccess
    {
        private SensorWatcher watcher;

        public SensorFaultHandler Faulted
        {
            get { return watcher.Faulted; }
            set { watcher.Faulted = value; }
        }

        public SensorAccess(SensorWatcher watcher)
        {
            this.watcher = watcher;
        }

        public bool IsHardwareConnected()
        {
            return watcher.HasSensors;
        }

        public Sensor GetSensor(SensorId id)
        {
            return watcher.GetSensor(id);
        }

        public void ClearSensorFaultState(SensorId id)
        {
            var s = GetSensor(id);
            s.FaultState = SensorFaultState.Normal;
        }

        public float GetSensorBias(SensorId id)
        {
            int i = (int)id;
            if (i >= 0 && i < watcher.BiasValues.Length)
                return watcher.BiasValues[i];
            return 0.0f;
        }

        public void SetSensorBias(SensorId id, float val)
        {
            int i = (int)id;
            if (i >= 0 && i < watcher.BiasValues.Length)
                watcher.BiasValues[i] = val;
        }
    }
}
