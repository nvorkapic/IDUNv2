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
            set { _curTemplate = value;  Notify(); }
        }

        public TemplatesViewModel()
        {
            Templates = AppData.GetFaultReportTemplates();
            CurTemplate = Templates.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            //Templates = AppData.GetFaultReportTemplates();
            DiscoveryList = await AppData.CloudClient.GetWorkOrderDiscCodes();
            SymptomList = await AppData.CloudClient.GetWorkOrderSymptCodes();
            PriorityList = await AppData.CloudClient.GetMaintenancePriorities();
        }
    }
}
