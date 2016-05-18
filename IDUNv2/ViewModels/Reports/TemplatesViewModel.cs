using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels.Reports
{
    public class TemplatesViewModel : BaseViewModel
    {
        public List<WorkOrderDiscCode> DiscoveryList { get; set; }
        public List<WorkOrderSymptCode> SymptomList { get; set; }
        public List<MaintenancePriority> PriorityList { get; set; }
        public List<TemplateModel> Templates { get; set; }

        private TemplateModel _curTemplate;
        public TemplateModel CurTemplate
        {
            get { return _curTemplate; }
            set { SetFaultCodes(value); _curTemplate = value;  Notify(); }
        }

        private void SetFaultCodes(TemplateModel val)
        {
            CurDiscovery = val.Discovery;
            CurSymptom = val.Symptom;
            CurPriority = val.Priority;
        }

        private WorkOrderDiscCode _curDiscovery;
        public WorkOrderDiscCode CurDiscovery
        {
            get { return _curDiscovery; }
            set { _curDiscovery = value; Notify(); }
        }

        private WorkOrderSymptCode _curSymptom;
        public WorkOrderSymptCode CurSymptom
        {
            get { return _curSymptom; }
            set { _curSymptom = value; Notify(); }
        }

        private MaintenancePriority _curPriority;
        public MaintenancePriority CurPriority
        {
            get { return _curPriority; }
            set { _curPriority = value; Notify(); }
        }

        public TemplatesViewModel()
        {
            Templates = AppData.FaultReports.GetFaultReportTemplates();
            CurTemplate = Templates.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            //DiscoveryList = await AppData.CloudClient.GetWorkOrderDiscCodes();
            //SymptomList = await AppData.CloudClient.GetWorkOrderSymptCodes();
            //PriorityList = await AppData.CloudClient.GetMaintenancePriorities();
            DiscoveryList = await AppData.FaultReports.GetDiscCodes();
            SymptomList = await AppData.FaultReports.GetSymptCodes();
            PriorityList = await AppData.FaultReports.GetPrioCodes();

            Templates[0].Discovery = DiscoveryList[0];
            Templates[1].Discovery = DiscoveryList[1];
            Templates[2].Discovery = DiscoveryList[2];

            CurTemplate = Templates[0];
        }
    }
}
