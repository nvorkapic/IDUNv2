using IDUNv2.Models;
using IDUNv2.SensorLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    public interface ISensorTriggerAccess
    {
        Task<List<SensorTrigger>> GetSensorTriggersFor(SensorId id);
        Task<SensorTrigger> SetSensorTrigger(SensorTrigger trigger);
        Task<SensorTrigger> DeleteSensorTrigger(SensorTrigger trigger);
        Task<SensorTrigger> FindSensorTrigger(int Id);
    }
}
