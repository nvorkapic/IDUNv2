using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.Data;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class ReportsPageViewModel : NotifyBase
    {
        private ReportTemplateViewModel _selectedTemplate;

        public ActionCommand<object> SaveCommand { get; private set; }

        public List<WorkOrderDiscCode> DiscoveryList { get { return AppData.GetWorkOrderDiscCodes(); } }
        public List<WorkOrderSymptCode> SymptomList { get { return AppData.GetWorkOrderSymptCodes(); } }
        public List<MaintenancePriority> PriorityList { get { return AppData.GetWorkOrderPrioCodes(); } }
        public ObservableCollection<ReportTemplateViewModel> Templates { get; set; }

        public ReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value;  Notify(); }
        }

        private async void SaveCommand_Execute(object param)
        {
            SelectedTemplate.Model = await AppData.SetReportTemplate(SelectedTemplate.Model);
            SelectedTemplate.Dirty = false;
        }

        public ReportsPageViewModel()
        {
            SaveCommand = new ActionCommand<object>(SaveCommand_Execute);
        }

        public async Task InitAsync()
        {
            await AppData.FillCaches();
            var temp = await AppData.GetReportTemplates();
            Templates = new ObservableCollection<ReportTemplateViewModel>(temp.Select(t => new ReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        public void CreateTemplate()
        {
            SelectedTemplate = new ReportTemplateViewModel(new ReportTemplate { Name = "#New Template" });
            SelectedTemplate.Dirty = true;
            Templates.Add(SelectedTemplate);
        }

        //public void RemoveTemplate(ReportTemplateViewModel Template)
        //{
        //    Templates.Remove(Template);
        //}
    }
}
