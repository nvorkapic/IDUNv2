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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Directive { get; set; }
        public string FaultDescr { get; set; }
        public WorkOrderDiscCode Discovery { get; set; }
        public WorkOrderSymptCode Symptom { get; set; }
        public MaintenancePriority Priority { get; set; }
    }
}
