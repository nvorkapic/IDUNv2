using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public class ReportTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(64), NotNull]
        public string SensorId { get; set; }

        [MaxLength(64), NotNull]
        public string Name { get; set; }

        public string Directive { get; set; }
        public string FaultDescr { get; set; }
        public string DiscCode { get; set; }
        public string SymptCode { get; set; }
        public string PrioCode { get; set; }
    }
}
