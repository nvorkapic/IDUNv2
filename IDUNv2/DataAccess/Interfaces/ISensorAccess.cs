using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    public interface ISensorAccess
    {
        SensorFaultHandler Faulted { get; set; }

        bool IsHardwareConnected();
        Sensor GetSensor(SensorId id);
        void ClearSensorFaultState(SensorId id);
        float GetSensorBias(SensorId id);
        void SetSensorBias(SensorId id, float val);
    }
}
