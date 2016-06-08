using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private SensorTriggerComparer _comparer = SensorTriggerComparer.Below;

        public SensorTriggerComparer Comparer
        {
            get { return _comparer; }
            set
            {
                _comparer = value;
                Notify("ComparerBelow");
                Notify("ComparerAbove");
            }
        }

        public bool ComparerBelow
        {
            get { return Comparer == SensorTriggerComparer.Below; }
            set { Comparer = SensorTriggerComparer.Below; }
        }

        public bool ComparerAbove
        {
            get { return Comparer == SensorTriggerComparer.Above; }
            set { Comparer = SensorTriggerComparer.Above; }
        }


        public SensorTrigger Model { get; set; }

        public SensorTriggerViewModel(SensorTrigger model)
        {
            Model = model;
            Comparer = model.Comparer;
        }

        public override string ToString()
        {
            return $"TriggerId: {Model.Id} ON value {Model.Comparer.ToString().ToUpper()} {Model.Value}";
        }
    }

    public class SensorSettingsViewModel : NotifyBase
    {
        private Sensor _sensor;
        private SensorTriggerViewModel _selectedTrigger;

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

        #region Command Bindings

        public ActionCommand<object> SaveCommand { get; set; }
        public ActionCommand<object> AddTriggerCommand { get; set; }

        #endregion

        public ObservableCollection<SensorTriggerViewModel> Triggers { get; set; }

        public SensorSettingsViewModel()
        {
            SaveCommand = new ActionCommand<object>(SaveCommand_Execute);
            AddTriggerCommand = new ActionCommand<object>(AddTriggerCommand_Execute);

            SelectedTrigger = new SensorTriggerViewModel(new SensorTrigger());

            var triggers = DAL.GetSensorTriggers().Result;
            Triggers = new ObservableCollection<SensorTriggerViewModel>(triggers.Select(t => new SensorTriggerViewModel(t)));
        }

        private void SaveCommand_Execute(object param)
        {
            Sensor.SaveToLocalSettings();
        }

        private void AddTriggerCommand_Execute(object param)
        {

        }
    }

    public sealed partial class SensorSettingsPage : Page
    {
        private SensorSettingsViewModel viewModel = new SensorSettingsViewModel();

        public SensorSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.Sensor = e.Parameter as Sensor;
            //viewModel.Sensor = DAL.SensorWatcher.TemperatureSensor;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(sender as TextBox);
            osk.Visibility = Visibility.Visible;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            osk.SetTarget(null as TextBox);
            osk.Visibility = Visibility.Collapsed;
        }
    }
}
