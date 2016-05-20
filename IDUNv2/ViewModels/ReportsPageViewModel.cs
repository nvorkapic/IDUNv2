using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class ReportsPageViewModel : ViewModelBase
    {
        private ReportTemplateViewModel _selectedTemplate;

        public List<WorkOrderDiscCode> DiscoveryList { get; set; }
        public List<WorkOrderSymptCode> SymptomList { get; set; }
        public List<MaintenancePriority> PriorityList { get; set; }
        public ObservableCollection<ReportTemplateViewModel> Templates { get; set; }

        public ReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value;  Notify(); }
        }

        public ReportsPageViewModel()
        {
            Templates = new ObservableCollection<ReportTemplateViewModel>
                (AppData.FaultReports.GetFaultReportTemplates().Select(t => new ReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            DiscoveryList = await AppData.FaultReports.GetDiscCodes();
            SymptomList = await AppData.FaultReports.GetSymptCodes();
            PriorityList = await AppData.FaultReports.GetPrioCodes();

            Templates[0].Discovery = DiscoveryList[0];
            Templates[1].Discovery = DiscoveryList[1];
            Templates[2].Discovery = DiscoveryList[2];

            SelectedTemplate = Templates[0];
        }

        public void CreateTemplate()
        {
            var model = new ReportTemplate { Name = "#New Template" };
            SelectedTemplate = new ReportTemplateViewModel(model);
            Templates.Add(SelectedTemplate);
        }
    }
}
