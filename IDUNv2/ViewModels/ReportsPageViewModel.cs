using Addovation.Cloud.Apps.AddoResources.Client.Portable;
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
    public class ReportsPageViewModel : ViewModelBase
    {
        private ReportService _reports;
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

        public ReportsPageViewModel(ReportService reports)
        {
            _reports = reports;
            Templates = new ObservableCollection<ReportTemplateViewModel>
                (_reports.GetTemplates().Result.Select(t => new ReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
            Templates.CollectionChanged += Templates_CollectionChanged;
        }

        private async void Templates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var vm = e.NewItems.Cast<ReportTemplateViewModel>().SingleOrDefault();
            if (vm != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    await _reports.SetTemplate(vm.Model);
            }
        }

        public async Task InitAsync()
        {
            DiscoveryList = await _reports.GetDiscCodes();
            SymptomList = await _reports.GetSymptCodes();
            PriorityList = await _reports.GetPrioCodes();

            try
            {
                Templates[0].Discovery = DiscoveryList[0];
                Templates[1].Discovery = DiscoveryList[1];
                Templates[2].Discovery = DiscoveryList[2];
                SelectedTemplate = Templates[0];
            }
            catch (Exception)
            {
            }  
        }

        public void CreateTemplate()
        {
            var model = new ReportTemplate { Name = "#New Template" };
            SelectedTemplate = new ReportTemplateViewModel(model);
            Templates.Add(SelectedTemplate);
        }
    }
}
