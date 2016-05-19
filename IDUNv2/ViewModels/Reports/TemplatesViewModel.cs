using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models.Reports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<TemplateModel> Templates { get; set; }

        private TemplateModel _curTemplate;
        public TemplateModel CurTemplate
        {
            get { return _curTemplate; }
            set { _curTemplate = value; Notify(); }
        }

        public TemplatesViewModel()
        {
            Templates = new ObservableCollection<TemplateModel>(AppData.FaultReports.GetFaultReportTemplates());
            CurTemplate = Templates.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            DiscoveryList = await AppData.FaultReports.GetDiscCodes();
            SymptomList = await AppData.FaultReports.GetSymptCodes();
            PriorityList = await AppData.FaultReports.GetPrioCodes();

            Templates[0].Discovery = DiscoveryList[0];
            Templates[1].Discovery = DiscoveryList[1];
            Templates[2].Discovery = DiscoveryList[2];

            CurTemplate = Templates[0];
        }

        public void CreateTemplate()
        {
            var template = new Models.Reports.TemplateModel { Name = "#New Template" };
            Templates.Add(AppData.FaultReports.AddTemplate(template));
        }
    }
}
