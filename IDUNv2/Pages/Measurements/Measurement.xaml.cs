using IDUNv2.Models;
using IDUNv2.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages.Measurements
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Measurement : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList = new ObservableCollection<MeasurementListSettingsItems>();


        public Measurement()
        {
            this.InitializeComponent();
            this.Loaded += Measurement_Loaded;
        }

        private async void Measurement_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("MeasurementConfiguration.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
                MeasurementConfigurationList = JsonConvert.DeserializeObject<ObservableCollection<MeasurementListSettingsItems>>(ConfigText);

                foreach (var item in MeasurementConfigurationList)
                {
                    MeasurementViewModel.Measurements.Add(new MeasurementModel { MeasurementName = item.Title, Enabled = item.Config.Enabled, ThresholdList = item.Config.Thresholds });
                }
                MeasurementModel CurrentMeasurement = MeasurementViewModel.Measurements.Where(x => x.MeasurementName == MainPage.SubMenI.Label).FirstOrDefault();
                this.DataContext = CurrentMeasurement;
            }
            catch
            {
                foreach(var s in Services.ConfigService.Sensors)
                {
                    MeasurementViewModel.Measurements.Add(new MeasurementModel { MeasurementName = Common.Assets.SensorIcons[s.Type].Item3 });
                }
            }


        }


    }
}
