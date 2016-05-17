using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models.Reports
{
    public class TemplateModel
    {
        public string Name { get; set; }
        public string Directive { get; set; }
        public string FaultDescr { get; set; }
        public WorkOrderDiscCode Discovery { get; set; }
        public WorkOrderSymptCode Symptom { get; set; }
        public MaintenancePriority Priority { get; set; }
    }
}
