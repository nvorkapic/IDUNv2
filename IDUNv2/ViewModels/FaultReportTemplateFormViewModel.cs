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
        #region Notify Fields

        private ObservableCollection<FaultReportTemplateViewModel> _templates;
        private FaultReportTemplateViewModel _selectedTemplate;

        #endregion

        #region Notify Properties

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

        #endregion

        #region Properties

        public List<WorkOrderDiscCode> DiscoveryList { get; private set; }
        public List<WorkOrderSymptCode> SymptomList { get; private set; }
        public List<MaintenancePriority> PriorityList { get; private set; }
        public ICollection<CmdBarItem> CmdBarItems { get; private set; }

        #endregion

        #region CmdBar Actions

        private void CreateTemplate(object param)
        {
            var template = new FaultReportTemplateViewModel(new FaultReportTemplate { Name = "#New Template" });
            Templates.Add(template);
            SelectedTemplate = template;
            SelectedTemplate.Dirty = true;
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

        private async void DeleteTemplate(object param)
        {
            bool success = await DAL.DeleteFaultReportTemplate(SelectedTemplate.Model);
            if (success)
            {
                ShellPage.Current.AddNotificatoin(NotificationType.Information,
                    "Template Deleted", $"Successfully deleted template with id: {SelectedTemplate.Model.Id}");
                if (Templates.Remove(SelectedTemplate))
                {
                    SelectedTemplate = Templates.LastOrDefault();
                }
            }
            else
            {
                ShellPage.Current.AddNotificatoin(NotificationType.Error,
                    "Template Delete Error", $"Could not delete template with id: {SelectedTemplate.Model.Id} because it's in use.");
            }
        }

        #endregion

        #region Constructors

        public FaultReportTemplateFormViewModel()
        {
            CmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Add, "Create", CreateTemplate),
                new CmdBarItem(Symbol.Save, "Save", SaveTemplate),
                new CmdBarItem(Symbol.Delete, "Delete", DeleteTemplate),
            };
        }

        #endregion

        #region Lazy Init

        public async Task InitAsync()
        {
            await DAL.FillCaches();
            try
            {
                DiscoveryList = DAL.GetWorkOrderDiscCodes();
                SymptomList = DAL.GetWorkOrderSymptCodes();
                PriorityList = DAL.GetWorkOrderPrioCodes();
            }
            catch
            {

            }
            var temp = await DAL.GetFaultReportTemplates();
            Templates = new ObservableCollection<FaultReportTemplateViewModel>(temp.Select(t => new FaultReportTemplateViewModel(t)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        #endregion
    }
}
