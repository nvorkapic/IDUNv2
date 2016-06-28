using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    /// <summary>
    /// Machine definition for a particular Object ID
    /// </summary>
    public class Machine
    {
        public string MchCode { get; set; }
        public string MchCodeContract { get; set; }
        public string OrgCode { get; set; }

        /// <summary>
        /// Statically known objects known to work.
        /// </summary>
        public static Dictionary<string, Machine> Machines { get; } = new Dictionary<string, Machine>
        {
            { "6600-1", new Machine { MchCode = "6600-1", MchCodeContract = "1", OrgCode = "100" } },
            { "800C", new Machine { MchCode = "800C", MchCodeContract = "3", OrgCode = "100" } },
            { "VPLHS50-SNY933556", new Machine { MchCode = "VPLHS50-SNY933556", MchCodeContract = "501", OrgCode = "100" } },
            { "10326823-333", new Machine { MchCode = "10326823-333", MchCodeContract = "70", OrgCode = "100" } },
        };

        public override string ToString()
        {
            return MchCode;
        }
    }
}
