using IDUNv2.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Services
{
    public class SensorService
    {
        private static SQLiteConnection db = new SQLiteConnection(AppData.SqlitePlatform, AppData.DbPath);


        public SensorService()
        {
            db.CreateTable<SensorTrigger>();
        }

        public Task<List<SensorTrigger>> GetTriggers()
        {
            var triggers = db.Table<SensorTrigger>().ToList();
            return Task.FromResult(triggers);
        }

        public Task<SensorTrigger> SetTemplate(SensorTrigger trigger)
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
    }
}
