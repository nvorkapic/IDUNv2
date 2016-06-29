using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    /// <summary>
    /// Machine definition for a particular Object ID
    /// </summary>
    public class Machine
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string MchCode { get; set; }
        [NotNull]
        public string MchCodeContract { get; set; }
        [NotNull]
        public string OrgCode { get; set; }

        public override string ToString()
        {
            return MchCode;
        }

        public override bool Equals(object obj)
        {
            return MchCode == (obj as Machine).MchCode;
        }

        public override int GetHashCode()
        {
            return MchCode.GetHashCode();
        }
    }
}
