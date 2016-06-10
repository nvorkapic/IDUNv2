using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public class SensorTriggerViewModel : NotifyBase
    {
        #region Notify Fields

        private SensorTriggerComparer _comparer = SensorTriggerComparer.Below;
        private SensorTrigger _model;

        #endregion

        #region Notify Properties

        public SensorTriggerComparer Comparer
        {
            get { return _comparer; }
            set
            {
                _comparer = value;
                Model.Comparer = value;
                Notify("ComparerBelow");
                Notify("ComparerAbove");
            }
        }

        public bool ComparerBelow
        {
            get { return Comparer == SensorTriggerComparer.Below; }
            set { Comparer = SensorTriggerComparer.Below;}
        }

        public bool ComparerAbove
        {
            get { return Comparer == SensorTriggerComparer.Above; }
            set { Comparer = SensorTriggerComparer.Above; }
        }

        public SensorTrigger Model
        {
            get { return _model; }
            set { _model = value; Notify(); }
        }

        public int Id
        {
            get { return Model.Id; }
            set { Model.Id = value; Notify(); }
        }

        public int TemplateId
        {
            get { return Model.TemplateId; }
            set { Model.TemplateId = value; Notify(); }
        }

        public float Value
        {
            get { return Model.Value; }
            set { Model.Value = value; Notify(); }
        }

        #endregion

        public SensorTriggerViewModel(SensorTrigger model)
        {
            Model = model;
            Comparer = model.Comparer;

        }

        public override string ToString()
        {
            if (Model.TemplateId == 0)
                return $"Existing Trigger Not Configured: Enter and Save Changes!";
            else
                return $"TriggerId: {Model.Id} ON value {Model.Comparer.ToString().ToUpper()} {Model.Value} with TemplateID {Model.TemplateId}";
        }
    }

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

        #endregion

        public List<FaultReportTemplate> Templates { get; set; }
        
        public SensorSettingsViewModel()
        {
            NavigationItems();
            SelectedTrigger = new SensorTriggerViewModel(new SensorTrigger());
            Templates = new List<FaultReportTemplate>();
            var triggers = DAL.GetSensorTriggers().Result;
            Triggers = new ObservableCollection<SensorTriggerViewModel>(triggers.Select(t => new SensorTriggerViewModel(t)));
        }

        private void SaveCommand_Execute(object param)
        {
            Sensor.SaveToLocalSettings();
        }

        private async void CreateTriggerCommand_Execute(object param)
        {
           try
            {
                SelectedTrigger = new SensorTriggerViewModel( new SensorTrigger {});
                SelectedTrigger.Model = await DAL.SetSensorTrigger(SelectedTrigger.Model);
                Triggers.Add(new SensorTriggerViewModel(SelectedTrigger.Model));
            }
            catch
            {

            }
        }

        private async void SaveChangesTriggerCommand_Execute(object param)
        {
            if (SelectedTemplate == null)
            {
                return;
            }
            SelectedTrigger.TemplateId = SelectedTemplate.Id;
            SelectedTrigger.Model = await DAL.SetSensorTrigger(SelectedTrigger.Model);
            var triggers = DAL.GetSensorTriggers().Result;
            Triggers = new ObservableCollection<SensorTriggerViewModel>(triggers.Select(t => new SensorTriggerViewModel(t)));
        }

        private async void RemoveTriggerCommand_Execute(object param)
        {
            try
            {
                SelectedTrigger.Model = await DAL.DeleteSensorTrigger(SelectedTrigger.Model);
                Triggers.Remove(SelectedTrigger);
                SelectedTrigger = Triggers.LastOrDefault();
            }
            catch
            {
            }
        }

        public ICollection<CmdBarItem> GeneralItems { get; private set; }
        public ICollection<CmdBarItem> TriggerStandard { get; private set; }
        public ICollection<CmdBarItem> TriggerSelected { get; private set; }

        private void NavigationItems()
        {

            GeneralItems = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Save, "Save", SaveCommand_Execute)
            };
            TriggerSelected = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Delete, "Remove Trigger",RemoveTriggerCommand_Execute),
                new CmdBarItem(Symbol.Save, "Save Trigger", SaveChangesTriggerCommand_Execute),
                new CmdBarItem(Symbol.Add, "Create Trigger", CreateTriggerCommand_Execute)
            };
            TriggerStandard = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Add, "Create Trigger", CreateTriggerCommand_Execute)
            };
        }
    }

    public sealed partial class SensorSettingsPage : Page
    {
        private SensorSettingsViewModel viewModel = new SensorSettingsViewModel();

        public SensorSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += SensorSettingsPage_Loaded;
        }

        private async void SensorSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Templates = await DAL.GetFaultReportTemplates(); ;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.Sensor = e.Parameter as Sensor;
            DAL.PushNavLink(new NavLinkItem { Title = "Settings", PageType = GetType() });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DAL.SetCmdBarItems(null);
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);      
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        private void Templates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            var selectedItem = (FaultReportTemplate)cb.SelectedItem;
            viewModel.SelectedTrigger.TemplateId = selectedItem.Id;
        }

        private void Triggers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;

            if (viewModel.SelectedTrigger != null)
            {
                viewModel.SelectedTemplate = viewModel.Templates.SingleOrDefault(p => p.Id == viewModel.SelectedTrigger.TemplateId);
            }

            if (lb.SelectedIndex != -1)
            {
                DAL.SetCmdBarItems(null);
                DAL.SetCmdBarItems(viewModel.TriggerSelected);
                EditTriggerPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DAL.SetCmdBarItems(null);
                DAL.SetCmdBarItems(viewModel.TriggerStandard);
                EditTriggerPanel.Visibility = Visibility.Collapsed;
            }
            
        }

        private void Pivot_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            DAL.SetCmdBarItems(null);
            if (args.Item.Header.ToString() == "General")
                DAL.SetCmdBarItems(viewModel.GeneralItems);
            else
            {
                if (viewModel.SelectedTrigger != null)
                    DAL.SetCmdBarItems(viewModel.TriggerSelected);
                else
                    DAL.SetCmdBarItems(viewModel.TriggerStandard);
            }              
        }
    }
}
