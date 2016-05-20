using IDUNv2.Common;
using IDUNv2.Models;
using IDUNv2.Services;
using IDUNv2.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace IDUNv2.ViewModels
{
    public class MeasurementListSettingsItems
    {
        public SensorConfig Config { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }

        public ObservableCollection<Operator> ListAvailableOperators { get; set; }

        public MeasurementListSettingsItems(SensorType Type, string Icon, string Unit, string title)
        {
            this.Title = title;
            this.Config = ConfigService.GetSensorConfig(Type);
            this.Icon = Icon;
            this.Unit = Unit;
            ListAvailableOperators = new ObservableCollection<Operator> { Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual };
        }

    }



    public class MeasurementListSettingsVM : ViewModelBase
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public ObservableCollection<Models.Reports.TemplateModel> Templates { get; set; }

        public ObservableCollection<MeasurementListSettingsItems> _measurementConfigurationList = new ObservableCollection<MeasurementListSettingsItems>();

        public ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList { get { return _measurementConfigurationList;}}


        private MeasurementListSettingsItems _currentMeasurements;
        public MeasurementListSettingsItems CurrentMeasurements { get { return _currentMeasurements; } set { _currentMeasurements = value; Notify(); } }
        public MeasurementListSettingsVM()
        {
            InitializeConfigurationList();
            CurrentMeasurements = MeasurementConfigurationList.FirstOrDefault();
            Templates = new ObservableCollection<Models.Reports.TemplateModel>(AppData.FaultReports.GetFaultReportTemplates());
        }

        public async void SaveMCListToLocal()
        {
            string json = JsonConvert.SerializeObject(_measurementConfigurationList.ToArray(), Formatting.Indented);
            StorageFile ConfigFile = await localFolder.CreateFileAsync("MeasurementConfiguration.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(ConfigFile, json);
        }

        public async Task LoadMCListFromLocal()
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("MeasurementConfiguration.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
                _measurementConfigurationList = JsonConvert.DeserializeObject<ObservableCollection<MeasurementListSettingsItems>>(ConfigText);
                CurrentMeasurements = MeasurementConfigurationList.FirstOrDefault();
            }
            catch
            {

            }
        }
        public void InitializeConfigurationList()
        {
            foreach (var s in ConfigService.Sensors)
            {
                _measurementConfigurationList.Add(new MeasurementListSettingsItems(s.Type, Assets.SensorIcons[s.Type].Item1, Assets.SensorIcons[s.Type].Item2, Assets.SensorIcons[s.Type].Item3));
            }
        }
    }


   
}
