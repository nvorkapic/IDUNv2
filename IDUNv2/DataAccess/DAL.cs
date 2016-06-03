using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using Addovation.Common.Extensions;
using Addovation.Common.Models;
using IDUNv2.Models;
using IDUNv2.Pages;
using SenseHat;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Xamarin;

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
        private static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        private static readonly SQLiteConnection db = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
        public static readonly SensorWatcher SensorWatcher = new SensorWatcher(1);

        private static CachingCloudClient cloud;

        static DAL()
        {
            db.CreateTable<ReportTemplate>();
            db.CreateTable<SensorTrigger>();

            InitCloud();
        }

        public static async Task FillCaches()
        {
            await cloud.FillCaches();
        }

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

                var cloudUrl = CommonDictionary.CloudUrls[url];
                var connectionInfo = new ConnectionInfo(cloudUrl, systemid, username, password);

                cloud = new CachingCloudClient
                {
                    ConnectionInfo = connectionInfo,
                    SessionManager = new Addovation.Cloud.Apps.AddoResources.Client.Portable.SessionManager()
                };

                //InsightsHelper.Init();
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

        #region Reports

        public static Task<List<ReportTemplate>> GetReportTemplates()
        {
            var templates = db.Table<ReportTemplate>().ToList();
            return Task.FromResult(templates);
        }

        public static Task<ReportTemplate> FindReportTemplate(int Id)
        {
            var template = db.Find<ReportTemplate>(Id);
            return Task.FromResult(template);
        }

        public static Task<ReportTemplate> SetReportTemplate(ReportTemplate template)
        {
            if (template.Id == 0)
                db.Insert(template);
            else
                db.Update(template);
            return Task.FromResult(template);
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

        #region SensorTrigger

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
    }
}
