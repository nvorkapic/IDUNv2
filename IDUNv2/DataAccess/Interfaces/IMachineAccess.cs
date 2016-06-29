using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    public interface IMachineAccess
    {
        Task<List<Machine>> GetMachines();
        Task<Machine> FindMachine(string mchCode);
        Task<Machine> SetMachine(Machine machine);
        Task<bool> DeleteMachine(Machine machine);
    }
}
