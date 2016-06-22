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
        #region Fields

        private ISensorTriggerAccess triggerAccess;
        private IFaultReportAccess faultReportAccess;

        #endregion

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

        public SensorDeviceState SensorDeviceState
        {
            get { return Sensor.DeviceState; }
            set
            {
                Sensor.DeviceState = value;
                Notify("SensorDeviceStateOffline");
                Notify("SensorDeviceStateOnline");
                Notify("SensorDeviceStateSimulated");
            }
        }

        public bool SensorDeviceStateOffline
        {
            get { return SensorDeviceState == SensorDeviceState.Offline; }
            set { SensorDeviceState = SensorDeviceState.Offline; }
        }

        public bool SensorDeviceStateOnline
        {
            get { return SensorDeviceState == SensorDeviceState.Online; }
            set { SensorDeviceState = SensorDeviceState.Online; }
        }

        public bool SensorDeviceStateSimulated
        {
            get { return SensorDeviceState == SensorDeviceState.Simulated; }
            set { SensorDeviceState = SensorDeviceState.Simulated; }
        }

        public void SetSelectedTriggerFromTemplate(FaultReportTemplate template)
        {
            if (SelectedTrigger != null)
            {
                SelectedTrigger.TemplateId = template.Id;
            }
        }

        #endregion

        #region Properties

        public List<FaultReportTemplate> Templates { get; set; }
        public CmdBarItem[] GeneralCmdBarItems { get; private set; }
        public CmdBarItem[] TriggerCmdBarItems { get; private set; }

        #endregion

        #region CmdBar Actions

        private void SaveSensor(object param)
        {
            //var triggers = await triggerAccess.GetSensorTriggersFor(Sensor.Id);
            //var ts = triggers.Select(t => new Sensor.Trigger
            //{
            //    id = t.Id,
            //    cmp = t.Comparer == SensorTriggerComparer.Above ? 1 : -1,
            //    val = t.Value
            //}).ToArray();
            //Sensor.SetTriggers(ts);
            Sensor.SaveToLocalSettings();
            ShellPage.Current.AddNotificatoin(
                NotificationType.Information,
                "Sensor Saved",
                "Sensor " + Sensor.Id + " Settings Changes Saved!");
        }

        private void ClearSensor(object param)
        {
            Sensor.Clear();
            Notify("SensorDeviceStateOnline");
            Notify("SensorDeviceStateSimulated");
            Notify("SensorDeviceStateOffline");

            ShellPage.Current.AddNotificatoin(
                NotificationType.Information,
                "Sensor Cleared",
                Sensor.ToString());
        }

        private void ResetSensor(object param)
        {
            Sensor.SetDefaults();
            Notify("SensorDeviceStateOnline");
            Notify("SensorDeviceStateSimulated");
            Notify("SensorDeviceStateOffline");

            ShellPage.Current.AddNotificatoin(
                NotificationType.Information,
                "Sensor Reset",
                Sensor.ToString());
        }

        private void CreateTrigger(object param)
        {
            var trigger = new SensorTriggerViewModel(Sensor, new SensorTrigger { SensorId = Sensor.Id });
            Triggers.Add(trigger);
            SelectedTrigger = trigger;
        }

        private async void SaveTrigger(object param)
        {
            if (SelectedTemplate != null)
            {
                SelectedTrigger.TemplateId = SelectedTemplate.Id;
                SelectedTrigger.Model = await triggerAccess.SetSensorTrigger(SelectedTrigger.Model);
                Sensor.SetTrigger(SelectedTrigger.Id, SelectedTrigger.Value, SelectedTrigger.Comparer == SensorTriggerComparer.Above ? 1 : -1);

                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "Trigger Saved",
                    SelectedTrigger.ToString());
            }
        }

        private async void DeleteTrigger(object param)
        {
            if (SelectedTrigger != null)
            {
                SelectedTrigger.Model = await triggerAccess.DeleteSensorTrigger(SelectedTrigger.Model);
                Sensor.RemoveTrigger(new Sensor.Trigger { id = SelectedTrigger.Id });

                ShellPage.Current.AddNotificatoin(NotificationType.Information, "Trigger Deleted", SelectedTrigger.ToString());

                Triggers.Remove(SelectedTrigger);
                SelectedTrigger = Triggers.LastOrDefault();
            }
        }

        #endregion

        public void UpdateSelectedTemplate()
        {
            if (SelectedTrigger != null)
            {
                SelectedTemplate = Templates.SingleOrDefault(p => p.Id == SelectedTrigger.TemplateId);
            }
        }

        public async Task InitAsync()
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            Templates = await faultReportAccess.GetFaultReportTemplates();
            var triggers = await triggerAccess.GetSensorTriggersFor(Sensor.Id);
            Triggers = new ObservableCollection<SensorTriggerViewModel>(
                triggers.Select(t => new SensorTriggerViewModel(Sensor, t)));
            SelectedTrigger = Triggers.FirstOrDefault();
            ShellPage.SetSpinner(LoadingState.Finished);
        }

        #region Constructors

        public SensorSettingsViewModel(ISensorTriggerAccess triggerAccess, IFaultReportAccess faultReportAcccess)
        {
            this.triggerAccess = triggerAccess;
            this.faultReportAccess = faultReportAcccess;

            GeneralCmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Save, "Save", SaveSensor),
                new CmdBarItem(Symbol.Clear, "Clear", ClearSensor),
                new CmdBarItem(Symbol.GoToStart, "Defaults", ResetSensor)
            };

            TriggerCmdBarItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Add, "Create", CreateTrigger),
                new CmdBarItem(Symbol.Save, "Save", SaveTrigger),
                new CmdBarItem(Symbol.Delete, "Delete",DeleteTrigger),
            };
        }

        #endregion
    }
}
