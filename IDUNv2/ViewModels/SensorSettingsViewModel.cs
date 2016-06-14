using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.Pages;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.ViewModels
{
    public class SensorSettingsViewModel : NotifyBase
    {
        #region Notify Fields

        private Sensor _sensor;
        private SensorTriggerViewModel _selectedTrigger;
        private FaultReportTemplate _selectedTemplate;
        private ObservableCollection<SensorTriggerViewModel> _triggers;

        #endregion

        #region Notify Properties

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public SensorTriggerViewModel SelectedTrigger
        {
            get { return _selectedTrigger; }
            set { _selectedTrigger = value; Notify(); }
        }

        public FaultReportTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; Notify(); }
        }

        public ObservableCollection<SensorTriggerViewModel> Triggers
        {
            get { return _triggers; }
            set { _triggers = value; Notify(); }
        }

        public SensorState SensorState
        {
            get { return Sensor.State; }
            set
            {
                Sensor.State = value;
                Notify("SensorStateOffline");
                Notify("SensorStateOnline");
                Notify("SensorStateSimulated");
            }
        }

        public bool SensorStateOffline
        {
            get { return SensorState == SensorState.Offline; }
            set { SensorState = SensorState.Offline; }
        }

        public bool SensorStateOnline
        {
            get { return SensorState == SensorState.Online; }
            set { SensorState = SensorState.Online; }
        }

        public bool SensorStateSimulated
        {
            get { return SensorState == SensorState.Simulated; }
            set { SensorState = SensorState.Simulated; }
        }

        #endregion

        #region Properties

        public List<FaultReportTemplate> Templates { get; set; }
        public CmdBarItem[] GeneralCmdBarItems { get; private set; }
        public CmdBarItem[] TriggerCmdBarItems { get; private set; }

        #endregion

        private void SaveSensor(object param)
        {
            Sensor.SaveToLocalSettings();
            ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Sensor Saved", "Sensor "+Sensor.Id+" Settings Changes Saved!");
        }

        private void CreateTrigger(object param)
        {
            var trigger = new SensorTriggerViewModel(new SensorTrigger());
            Triggers.Add(trigger);
            SelectedTrigger = trigger;
            ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Trigger Created", "Empty Trigger Created.");
        }

        private async void SaveTrigger(object param)
        {
            if (SelectedTemplate != null)
            {
                SelectedTrigger.TemplateId = SelectedTemplate.Id;
                SelectedTrigger.Model = await DAL.SetSensorTrigger(SelectedTrigger.Model);

                string NotificationDescription = "Trigger Id: " + SelectedTrigger.Model.Id + " has had its' changes saved.\nComparer: " + SelectedTrigger.Model.Comparer + "\nValue: " + SelectedTrigger.Model.Value + "\nTemplate Id: " + SelectedTrigger.Model.TemplateId;
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Trigger Saved", NotificationDescription);

            }
        }

        private async void DeleteTrigger(object param)
        {
            if (SelectedTrigger != null)
            {

                SelectedTrigger.Model = await DAL.DeleteSensorTrigger(SelectedTrigger.Model);

                string NotificationDescription = "Trigger Id: " + SelectedTrigger.Model.Id + " has been deleted.\nComparer: " + SelectedTrigger.Model.Comparer + "\nValue: " + SelectedTrigger.Model.Value + "\nTemplate Id: " + SelectedTrigger.Model.TemplateId;
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Trigger Deleted", NotificationDescription);

                Triggers.Remove(SelectedTrigger);
                SelectedTrigger = Triggers.LastOrDefault();
            }
        }

        public async Task InitAsync()
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            Templates = await DAL.GetFaultReportTemplates();
            var triggers = await DAL.GetSensorTriggers();
            Triggers = new ObservableCollection<SensorTriggerViewModel>(triggers.Select(t => new SensorTriggerViewModel(t)));
            SelectedTrigger = Triggers.FirstOrDefault();
            ShellPage.SetSpinner(LoadingState.Finished);
        }

        public SensorSettingsViewModel()
        {
            GeneralCmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Save, "Save", SaveSensor)
            };

            TriggerCmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Delete, "Delete",DeleteTrigger),
                new CmdBarItem(Symbol.Save, "Save", SaveTrigger),
                new CmdBarItem(Symbol.Add, "Create", CreateTrigger)
            };
        }

        public void UpdateSelectedTemplate()
        {
            if (SelectedTrigger != null)
            {
                SelectedTemplate = Templates.SingleOrDefault(p => p.Id == SelectedTrigger.TemplateId);
            }
        }
    }
}
