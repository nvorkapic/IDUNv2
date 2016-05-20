using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using IDUNv2.Models;

namespace IDUNv2.Services
{
    public static class ConfigService
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        private static Models.SensorConfig[] sensors = new Models.SensorConfig[]
        {
            new Models.SensorConfig(SensorType.Usage),
            new Models.SensorConfig(SensorType.Temperature),
            new Models.SensorConfig(SensorType.Pressure),
            new Models.SensorConfig(SensorType.Humidity),
            new Models.SensorConfig(SensorType.Accelerometer),
            new Models.SensorConfig(SensorType.Magnetometer),
            new Models.SensorConfig(SensorType.Gyroscope)
        };

        internal static SensorConfig GetSensorConfig(SensorType type)
        {
            return sensors[(int)type];
        }

        public static Models.SensorConfig[] Sensors { get { return sensors; }}

        public static async Task Save()
        {
            string json = JsonConvert.SerializeObject(sensors, Formatting.Indented);
            StorageFile ConfigFile = await localFolder.CreateFileAsync("SensorConfig.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(ConfigFile, json);
        }

        public static async Task Load()
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("SensorConfig.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
                sensors = JsonConvert.DeserializeObject<Models.SensorConfig[]>(ConfigText);
            }
            catch
            {

            }
        }
    }

}
