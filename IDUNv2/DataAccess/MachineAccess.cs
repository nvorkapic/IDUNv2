using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models;
using SQLite.Net;

namespace IDUNv2.DataAccess
{
    public class MachineAccess : IMachineAccess
    {
        private SQLiteConnection db;

        public MachineAccess(SQLiteConnection db)
        {
            this.db = db;
        }

        public Task<List<Machine>> GetMachines()
        {
            var machines = db.Table<Machine>().ToList();
            return Task.FromResult(machines);
        }

        public Task<Machine> FindMachine(string mchCode)
        {
            var machine = db.Find<Machine>(m => m.MchCode == mchCode);
            return Task.FromResult(machine);
        }

        public Task<Machine> SetMachine(Machine machine)
        {
            if (machine.Id == 0)
            {
                db.Insert(machine);
            }
            else
            {
                db.Update(machine);
            }
            return Task.FromResult(machine);
        }

        public Task<bool> DeleteMachine(Machine machine)
        {
            int result = db.Delete(machine);
            return Task.FromResult(result == 1);
        }
    }
}
