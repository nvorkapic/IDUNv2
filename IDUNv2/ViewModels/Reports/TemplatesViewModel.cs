using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels.Reports
{
    public class TemplatesViewModel : ViewModelBase
    {
        public List<WorkOrderDiscCode> DiscoveryList { get; set; }
        public List<WorkOrderSymptCode> SymptomList { get; set; }
        public List<MaintenancePriority> PriorityList { get; set; }
        public ObservableCollection<ReportTemplate> Templates { get; set; }

        private ReportTemplate _curTemplate;
        public ReportTemplate CurTemplate
        {
            get { return _curTemplate; }
            set { _curTemplate = value; Notify(); }
        }

        public RelayCommand SaveCommand { get; private set; }

        public TemplatesViewModel()
        {
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            Templates = new ObservableCollection<ReportTemplate>(AppData.FaultReports.GetFaultReportTemplates());
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
            var template = new ReportTemplate { Name = "#New Template" };
            Templates.Add(template);
            CurTemplate = template;
        }

        private void ExecuteSaveCommand()
        {
            throw new NotImplementedException();
        }
    }
}
