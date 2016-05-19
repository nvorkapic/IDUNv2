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
    public class MeasurementSetting : BaseViewModel
    {
        public bool Enabled { get; set; }
        public ObservableCollection<Thresholds> Threshold { get; set; }
    }


    public class MeasurementListSettingsItems
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public MeasurementSetting Setting { get; set; }


        public ObservableCollection<Operator> ListAvailableOperators { get; set; }
        public ObservableCollection<Template> ListAvailableTemplates { get; set; }

        public MeasurementListSettingsItems(string Title, string Icon, string Unit)
        {
            this.Title = Title;
            this.Icon = Icon;
            this.Unit = Unit;
            Setting = new MeasurementSetting { Enabled = false, Threshold = new ObservableCollection<Thresholds>() };
            ListAvailableTemplates = new ObservableCollection<Template> { Template.None, Template.Quick, Template.Fault };
            ListAvailableOperators = new ObservableCollection<Operator> { Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual };
        }
    }

    public class Thresholds
    {

        public Operator? Operator { get; set; }

        public double Value { get; set; }

        public Template? Template { get; set; }


    }

    public enum Operator
    {
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal
    }

    public enum Template
    {
        Quick,
        Fault,
        None
    }

    public class MeasurementListSettingsVM : BaseViewModel
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public ObservableCollection<MeasurementSetting> _measurementSettingList = new ObservableCollection<MeasurementSetting>();


        public ObservableCollection<MeasurementListSettingsItems> _measurementConfigurationList = new ObservableCollection<MeasurementListSettingsItems>()
        {

            new MeasurementListSettingsItems ("Usage", "/Assets/Finger.png",""),
            new MeasurementListSettingsItems ("Temperature", "/Assets/Thermometer.png","°C"),
            new MeasurementListSettingsItems ("Pressure", "/Assets/Pressure.png", "kPa"),
            new MeasurementListSettingsItems ("Humidity", "/Assets/Humidity.png","%"),
            new MeasurementListSettingsItems ("Accelerometer","/Assets/Accelerometer.png","m/s²"),
            new MeasurementListSettingsItems ("Magnetometer", "/Assets/Magnet.png","μT"),
            new MeasurementListSettingsItems ("Gyroscope", "/Assets/Gyroscope.png","rad/s")
        };

        public ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList { get { return _measurementConfigurationList;}}

        //public ObservableCollection<MeasurementSetting> MeasurementSettingList { get { return _measurementSettingList; } }

        private MeasurementListSettingsItems _currentMeasurements;
        public MeasurementListSettingsItems CurrentMeasurements { get { return _currentMeasurements; } set { _currentMeasurements = value; Notify(); } }
        public MeasurementListSettingsVM()
        {
            CurrentMeasurements = MeasurementConfigurationList.FirstOrDefault();
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
    }


   
}
