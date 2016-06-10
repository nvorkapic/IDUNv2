using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.ViewModels
{
    public class FaultReportTemplateFormViewModel : NotifyBase
    {
        private ObservableCollection<FaultReportTemplateViewModel> _templates;
        private FaultReportTemplateViewModel _selectedTemplate;

        public List<WorkOrderDiscCode> DiscoveryList { get { return DAL.GetWorkOrderDiscCodes(); } }
        public List<WorkOrderSymptCode> SymptomList { get { return DAL.GetWorkOrderSymptCodes(); } }
        public List<MaintenancePriority> PriorityList { get { return DAL.GetWorkOrderPrioCodes(); } }

        public ICollection<CmdBarItem> CmdBarItems { get; private set; }

        public ObservableCollection<FaultReportTemplateViewModel> Templates
        {
            get { return _templates; }
            set { _templates = value; Notify(); }
        }

        public FaultReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; Notify(); }
        }

        private void CreateTemplate(object param)
        {
            SelectedTemplate = new FaultReportTemplateViewModel(new FaultReportTemplate { Name = "#New Template" });
            SelectedTemplate.Dirty = true;
            Templates.Add(SelectedTemplate);
            ShellPage.Current.AddNotificatoin(
                NotificationType.Information,
                "Template Created",
                "New Template has been added. Please configure and save to ensure proper functionality!");
        }

        private async void SaveTemplate(object param)
        {
            SelectedTemplate.Model = await DAL.SetFaultReportTemplate(SelectedTemplate.Model);
            SelectedTemplate.Dirty = false;
            ShellPage.Current.AddNotificatoin(
                NotificationType.Information,
                "Template Saved",
                "Template is configured and saved and ready for use!");
        }

        public FaultReportTemplateFormViewModel()
        {
            CmdBarItems = new CmdBarItem[]
            {
                // first item added will be to the right
                new CmdBarItem(Symbol.Delete, "Delete", o => { return; }),
                new CmdBarItem(Symbol.Save, "Save", SaveTemplate),

                // last item added will be the left most one
                new CmdBarItem(Symbol.Add, "Create", CreateTemplate),
            };
        }

        public async Task InitAsync()
        {
            await DAL.FillCaches();
            var temp = await DAL.GetFaultReportTemplates();
            Templates = new ObservableCollection<FaultReportTemplateViewModel>(temp.Select(t => new FaultReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
        }
    }
}
