using IDUNv2.Common;
using IDUNv2.Models;
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

    public class MeasurementViewModel : NotifyBase
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public static ObservableCollection<MeasurementModel> _measurements = new ObservableCollection<MeasurementModel>();
        public static ObservableCollection<MeasurementModel> Measurements { get { return _measurements; }}

        private MeasurementModel _currentMeasurement;
        public MeasurementModel CurrentMeasurement { get { return _currentMeasurement; } set { _currentMeasurement = value; Notify(); } }

        public static ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList { get; set; } = new ObservableCollection<MeasurementListSettingsItems>();

        public MeasurementViewModel()
        {
            InitializeMeasurementList();
            CurrentMeasurement = Measurements.FirstOrDefault();
        }

        public void InitializeMeasurementList()
        {
            foreach (var item in MeasurementConfigurationList)
            {
                Measurements.Add(new MeasurementModel { MeasurementName = item.Title, Enabled = item.Config.Enabled, ThresholdList = item.Config.Thresholds });
            }
        }

        public async Task LoadMCListFromLocal()
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("MeasurementConfiguration.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
                MeasurementConfigurationList = JsonConvert.DeserializeObject<ObservableCollection<MeasurementListSettingsItems>>(ConfigText);
            }
            catch
            {

            }
        }

    }
}
