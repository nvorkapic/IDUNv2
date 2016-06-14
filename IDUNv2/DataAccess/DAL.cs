using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using Addovation.Common.Extensions;
using Addovation.Common.Models;
using IDUNv2.Models;
using IDUNv2.Pages;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xamarin;
using Windows.UI.Core;

namespace IDUNv2.DataAccess
{
    public enum LoadingState
    {
        Idle,
        Loading,
        Finished
    }

    public static class DAL
    {
        private static readonly string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        private static readonly SQLiteConnection db = new SQLiteConnection(new SQLitePlatformWinRT(), dbPath);

        private static CachingCloudClient cloud;
        private static SensorWatcher sensorWatcher;

        static DAL()
        {
            db.CreateTable<FaultReportTemplate>();
            db.CreateTable<SensorTrigger>();

            InitCloud();
        }

        public static void SetDispatcher(CoreDispatcher dispatcher)
        {
            sensorWatcher = new SensorWatcher(dispatcher, 100);
            sensorWatcher.LoadSettings();
        }

        public static async Task FillCaches()
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            await cloud.FillCaches();
            ShellPage.SetSpinner(LoadingState.Finished);
        }

        #region Sensors

        public static bool HasSensors()
        {
            return sensorWatcher.HasSensors;
        }

        public static Sensor GetSensor(SensorId id)
        {
            return sensorWatcher.GetSensor(id);
        }

        public static void ClearSensorFaultState(SensorId id)
        {
            var s = GetSensor(id);
            s.Faulted = false;
        }

        public static float GetSensorBias(SensorId id)
        {
            int i = (int)id;
            if (i >= 0 && i < sensorWatcher.BiasValues.Length)
                return sensorWatcher.BiasValues[i];
            return 0.0f;
        }

        public static void SetSensorBias(SensorId id, float val)
        {
            int i = (int)id;
            if (i >= 0 && i < sensorWatcher.BiasValues.Length)
                sensorWatcher.BiasValues[i] = val;
        }

        #endregion

        #region Cloud

        private static void InitCloud()
        {
            try
            {
                string url = DeviceSettings.URL;
                string systemid = DeviceSettings.SystemID;
                string username = DeviceSettings.Username;
                string password = DeviceSettings.Password;

                url = url ?? "testcloud.addovation.com";
                systemid = systemid ?? "race8.addovation.com";
                username = username ?? "alex";
                password = password ?? "alex";

                if (!DeviceSettings.HasSettings())
                {
                    DeviceSettings.URL = url;
                    DeviceSettings.SystemID = systemid;
                    DeviceSettings.Username = username;
                    DeviceSettings.Password = password;
                }

                var cloudUrl = CommonDictionary.CloudUrls[url];
                var connectionInfo = new ConnectionInfo(cloudUrl, systemid, username, password);

                cloud = new CachingCloudClient
                {
                    ConnectionInfo = connectionInfo,
                    SessionManager = new Addovation.Cloud.Apps.AddoResources.Client.Portable.SessionManager()
                };

                InsightsHelper.Init();
                //InsightsHelper.SetUser(connectionInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FaultCodes

        public static List<WorkOrderDiscCode> GetWorkOrderDiscCodes()
        {
            return cloud.GetCachedWorkOrderDiscCodes();
        }

        public static List<WorkOrderSymptCode> GetWorkOrderSymptCodes()
        {
            return cloud.GetCachedWorkOrderSymptCodes();
        }

        public static List<MaintenancePriority> GetWorkOrderPrioCodes()
        {
            return cloud.GetCachedMaintenancePriorities();
        }

        public static WorkOrderDiscCode GetWorkOrderDiscovery(string discCode)
        {
            return cloud.GetCachedWorkOrderDiscCode(discCode);
        }

        public static WorkOrderSymptCode GetWorkOrderSymptom(string symptCode)
        {
            return cloud.GetCachedWorkOrderSymptCode(symptCode);
        }

        public static MaintenancePriority GetWorkOrderPiority(string prioCode)
        {
            return cloud.GetCachedMaintenancePriority(prioCode);
        }

        #endregion

        #region Fault Reports

        public static Task<List<FaultReportTemplate>> GetFaultReportTemplates()
        {
            var templates = db.Table<FaultReportTemplate>().ToList();
            return Task.FromResult(templates);
        }

        public static Task<FaultReportTemplate> FindFaultReportTemplate(int Id)
        {
            var template = db.Find<FaultReportTemplate>(Id);
            return Task.FromResult(template);
        }

        public static Task<FaultReportTemplate> SetFaultReportTemplate(FaultReportTemplate template)
        {
            if (template.Id == 0)
                db.Insert(template);
            else
                db.Update(template);
            return Task.FromResult(template);
        }

        public static Task<bool> DeleteFaultReportTemplate(FaultReportTemplate template)
        {
            //string sql = "SELECT id 
            var q = db.Table<SensorTrigger>().Where(sr => sr.TemplateId == template.Id);
            if (q.Count() != 0)
            {
                return Task.FromResult(false);
            }
            db.Delete(template);
            return Task.FromResult(true);
        }

        public static Task<List<FaultReport>> GetFaultReports()
        {
            return cloud.GetFaultReports();
        }

        public static Task<FaultReport> SetFaultReport(FaultReport report)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SensorTriggers

        public static Task<List<SensorTrigger>> GetSensorTriggers()
        {
            var triggers = db.Table<SensorTrigger>().ToList();
            return Task.FromResult(triggers);
        }

        public static Task<SensorTrigger> SetSensorTrigger(SensorTrigger trigger)
        {
            if (trigger.Id == 0)
            {
                db.Insert(trigger);
            }
            else
            {
                db.Update(trigger);
            }
            return Task.FromResult(trigger);
        }


        public static Task<SensorTrigger> DeleteSensorTrigger(SensorTrigger trigger)
        {
            db.Delete(trigger);
            return Task.FromResult(trigger);
        }

        public static Task<SensorTrigger> FindSensorTrigger(int Id)
        {
            var trigger = db.Find<SensorTrigger>(Id);
            return Task.FromResult(trigger);
        }

        //public static Task<SensorTrigger> InsertTrigger(SensorTrigger trigger)
        //{
        //    db.Insert(trigger);
        //    return Task.FromResult(trigger);

        //}

        #endregion

        #region Navigation

        public static void PushNavLink(NavLinkItem item)
        {
            ShellPage.Current.PushNavLink(item);
        }

        public static void PopNavLink()
        {
            ShellPage.Current.PopNavLink();
        }

        public static void SetCmdBarItems(ICollection<CmdBarItem> items)
        {
            var cmdBarItems = ShellPage.Current.CmdBarPrimaryCommands;
            cmdBarItems.Clear();
            if (items != null)
            {
                foreach (var item in items)
                {
                    cmdBarItems.Add(item.Btn);
                }
            }
        }

        #endregion

        #region OSK

        public static void ShowOSK(Control target)
        {
            ShellPage.Current.ShowOSK(target);
        }

        #endregion
    }
}
