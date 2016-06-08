using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.DataAccess;
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
    public class FaultReportTemplateFormViewModel : NotifyBase
    {
        private FaultReportTemplateViewModel _selectedTemplate;

        public ActionCommand<object> SaveCommand { get; private set; }

        public List<WorkOrderDiscCode> DiscoveryList { get { return DAL.GetWorkOrderDiscCodes(); } }
        public List<WorkOrderSymptCode> SymptomList { get { return DAL.GetWorkOrderSymptCodes(); } }
        public List<MaintenancePriority> PriorityList { get { return DAL.GetWorkOrderPrioCodes(); } }
        public ObservableCollection<FaultReportTemplateViewModel> Templates { get; set; }

        public FaultReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value;  Notify(); }
        }

        private async void SaveCommand_Execute(object param)
        {
            SelectedTemplate.Model = await DAL.SetFaultReportTemplate(SelectedTemplate.Model);
            SelectedTemplate.Dirty = false;
        }

        public FaultReportTemplateFormViewModel()
        {
            SaveCommand = new ActionCommand<object>(SaveCommand_Execute);
        }

        public async Task InitAsync()
        {
            await DAL.FillCaches();
            var temp = await DAL.GetFaultReportTemplates();
            Templates = new ObservableCollection<FaultReportTemplateViewModel>(temp.Select(t => new FaultReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        public void CreateTemplate()
        {
            SelectedTemplate = new FaultReportTemplateViewModel(new FaultReportTemplate { Name = "#New Template" });
            SelectedTemplate.Dirty = true;
            Templates.Add(SelectedTemplate);
        }

        //public void RemoveTemplate(ReportTemplateViewModel Template)
        //{
        //    Templates.Remove(Template);
        //}
    }
}
