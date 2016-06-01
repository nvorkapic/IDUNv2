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

        public ActionCommand<object> SaveCommand { get; private set; }

        public List<WorkOrderDiscCode> DiscoveryList { get { return _cache.DiscCodes; } }
        public List<WorkOrderSymptCode> SymptomList { get { return _cache.SymptCodes; } }
        public List<MaintenancePriority> PriorityList { get { return _cache.PrioCodes; } }
        public ObservableCollection<ReportTemplateViewModel> Templates { get; set; }

        public ReportTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value;  Notify(); }
        }

        private async void SaveCommand_Execute(object param)
        {
            SelectedTemplate.Model = await _reports.SetTemplate(SelectedTemplate.Model);
            SelectedTemplate.Dirty = false;
        }

        public ReportsPageViewModel(ReportService reports, FaultCodesCache cache)
        {
            SaveCommand = new ActionCommand<object>(SaveCommand_Execute);
            _cache = cache;
            _reports = reports;
        }

        public async Task InitAsync()
        {
            await _cache.InitAsync();
            Templates = new ObservableCollection<ReportTemplateViewModel>
                (_reports.GetTemplates().Result.Select(t => new ReportTemplateViewModel(t, _cache)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        public void CreateTemplate()
        {
            SelectedTemplate = new ReportTemplateViewModel(new ReportTemplate { Name = "#New Template" }, _cache);
            SelectedTemplate.Dirty = true;
            Templates.Add(SelectedTemplate);
        }
    }
}
