using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using SQLite.Net;

namespace IDUNv2.DataAccess
{
    public class SensorTriggerAccess : ISensorTriggerAccess
    {
        private SQLiteConnection db;

        public SensorTriggerAccess(SQLiteConnection db)
        {
            this.db = db;
        }

        public Task<List<SensorTrigger>> GetSensorTriggersFor(SensorId id)
        {
            var triggers = db.Table<SensorTrigger>()
                .Where(t => t.SensorId == id).ToList();
            return Task.FromResult(triggers);
        }

        public Task<SensorTrigger> SetSensorTrigger(SensorTrigger trigger)
        {
            if (trigger.Id == 0)
            {
                db.Insert(trigger);
            }
            else
            {
                db.Update(trigger);
            }
            return Task.FromResult(trigger);
        }

        public Task<SensorTrigger> DeleteSensorTrigger(SensorTrigger trigger)
        {
            db.Delete(trigger);
            return Task.FromResult(trigger);
        }

        public Task<SensorTrigger> FindSensorTrigger(int Id)
        {
            var trigger = db.Find<SensorTrigger>(Id);
            return Task.FromResult(trigger);
        }
    }
}
