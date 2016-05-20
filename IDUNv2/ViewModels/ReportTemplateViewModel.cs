using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class ReportTemplateViewModel : ViewModelBase
    {
        private ReportTemplate model;

        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; Notify(); }
        }

        public string Directive
        {
            get { return model.Directive; }
            set { model.Directive = value; Notify(); }
        }

        public string FaultDescr
        {
            get { return model.FaultDescr; }
            set { model.FaultDescr = value; Notify(); }
        }

        public WorkOrderDiscCode Discovery
        {
            get { return model.Discovery; }
            set { model.Discovery = value; Notify(); }
        }

        public WorkOrderSymptCode Symptom
        {
            get { return model.Symptom; }
            set { model.Symptom = value; Notify(); }
        }

        public MaintenancePriority Priority
        {
            get { return model.Priority; }
            set { model.Priority = value; Notify(); }
        }

        public ReportTemplateViewModel(ReportTemplate model)
        {
            this.model = model;
        }
    }
}
