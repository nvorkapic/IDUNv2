using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.Models;
using IDUNv2.Services;
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
        private ReportService _reports;
        private ReportTemplateViewModel _selectedTemplate;
        private FaultCodesCache _cache;

        public ActionCommand SaveCommand { get; private set; }

        public List<WorkOrderDiscCode> DiscoveryList { get { return _cache.DiscCodes; } }
        public List<WorkOrderSymptCode> SymptomList { get { return _cache.SymptCodes; } }
        public List<MaintenancePriority> PriorityList { get { return _cache.PrioCodes; } }
        public ObservableCollection<ReportTemplateViewModel> Templates { get; set; }

        public ReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value;  Notify(); }
        }

        private async void SaveCommand_Execute()
        {
            SelectedTemplate.Model = await _reports.SetTemplate(SelectedTemplate.Model);
            SelectedTemplate.Dirty = false;
        }

        public ReportsPageViewModel(ReportService reports, FaultCodesCache cache)
        {
            SaveCommand = new ActionCommand(SaveCommand_Execute);
            _cache = cache;
            _reports = reports;
            Templates = new ObservableCollection<ReportTemplateViewModel>
                (_reports.GetTemplates().Result.Select(t => new ReportTemplateViewModel(t, _cache)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            _cache = await FaultCodesCache.CreateAsync(AppData.CloudClient);
            SelectedTemplate = Templates.FirstOrDefault();
            //try
            //{
            //    Templates[0].Discovery = DiscoveryList[0];
            //    Templates[1].Discovery = DiscoveryList[1];
            //    Templates[2].Discovery = DiscoveryList[2];
            //    SelectedTemplate = Templates[0];
            //}
            //catch (Exception)
            //{
            //}  
        }

        public void CreateTemplate()
        {
            SelectedTemplate = new ReportTemplateViewModel(new ReportTemplate { Name = "#New Template" }, _cache);
            SelectedTemplate.Dirty = true;
            Templates.Add(SelectedTemplate);
        }
    }
}
