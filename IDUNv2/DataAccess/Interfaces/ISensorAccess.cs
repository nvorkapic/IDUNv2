using IDUNv2.SensorLib;

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
