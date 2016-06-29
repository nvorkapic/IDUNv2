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
        #region Fields

        private IFaultReportAccess faultReportAccess;

        #endregion

        #region Notify Fields

        private List<WorkOrderDiscCode> _discList;
        private List<WorkOrderSymptCode> _symptList;
        private List<MaintenancePriority> _prioList;
        private ObservableCollection<FaultReportTemplateViewModel> _templates;
        private FaultReportTemplateViewModel _selectedTemplate;

        #endregion

        #region Notify Properties

        public List<WorkOrderDiscCode> DiscoveryList
        {
            get { return _discList; }
            private set { _discList = value; Notify(); }
        }

        public List<WorkOrderSymptCode> SymptomList
        {
            get { return _symptList; }
            private set { _symptList = value; Notify(); }
        }

        public List<MaintenancePriority> PriorityList
        {
            get { return _prioList; }
            private set { _prioList = value; Notify(); }
        }

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

        public ICollection<CmdBarItem> CmdBarItems { get; private set; }

        #endregion

        #region CmdBar Actions

        private void CreateTemplate(object param)
        {
            var template = new FaultReportTemplateViewModel(
                new FaultReportTemplate { Name = "#New Template" },
                faultReportAccess);
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
            if (SelectedTemplate == null)
                return;

            if (SelectedTemplate.IsValidated)
            {
                SelectedTemplate.Model = await faultReportAccess.SetFaultReportTemplate(SelectedTemplate.Model);
                SelectedTemplate.Dirty = false;
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "Template Saved",
                    "Template is configured and saved and ready for use!");
            }
            else
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Error,
                    "Validation Error",
                    "You did not fill in all require fields for a FaultReportTemplate");
            }
        }

        private async void DeleteTemplate(object param)
        {
            if (SelectedTemplate == null)
                return;

            try
            {
                bool success = await faultReportAccess.DeleteFaultReportTemplate(SelectedTemplate.Model);
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
            catch (Exception ex)
            {
                ShellPage.Current.AddNotificatoin(NotificationType.Error, "Exception", ex.ToString());
            }
        }

        #endregion

        #region Constructors

        public FaultReportTemplateFormViewModel(IFaultReportAccess faultReportAccess)
        {
            this.faultReportAccess = faultReportAccess;

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
                DiscoveryList = faultReportAccess.GetWorkOrderDiscCodes();
                SymptomList = faultReportAccess.GetWorkOrderSymptCodes();
                PriorityList = faultReportAccess.GetWorkOrderPrioCodes();
            }
            catch
            {

            }
            var temp = await faultReportAccess.GetFaultReportTemplates();
            Templates = new ObservableCollection<FaultReportTemplateViewModel>(
                temp.Select(t => new FaultReportTemplateViewModel(t, faultReportAccess)));
            SelectedTemplate = Templates.FirstOrDefault();
        }

        #endregion
    }
}
