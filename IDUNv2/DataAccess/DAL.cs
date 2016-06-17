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

    /// <summary>
    /// Global application data access
    /// </summary>
    public static class DAL
    {
        private static readonly string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        private static readonly SQLiteConnection db = new SQLiteConnection(new SQLitePlatformWinRT(), dbPath);

        private static CachingCloudClient cloud;
        private static SensorWatcher sensorWatcher;

        public static ISensorAccess SensorAccess { get; private set; }
        public static ISensorTriggerAccess SensorTriggerAccess { get; private set; }
        public static IFaultReportAccess FaultReportAccess { get; private set; }


        static DAL()
        {
            db.CreateTable<FaultReportTemplate>();
            db.CreateTable<SensorTrigger>();
        }

        public static void Init(CoreDispatcher dispatcher)
        {
            sensorWatcher = new SensorWatcher(dispatcher, 100);
            sensorWatcher.LoadSettings();

            SensorAccess = new SensorAccess(sensorWatcher);
            SensorTriggerAccess = new SensorTriggerAccess(db);
            //FaultReportAccess = new FaultReportAccess(cloud, db);
            FaultReportAccess = new MockFaultReportAccess();
        }

        #region Cloud

        public static Task<bool> ConnectToCloud()
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

                return cloud.Authenticate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task FillCaches()
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            await FaultReportAccess.FillCaches();
            ShellPage.SetSpinner(LoadingState.Finished);
        }

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

        #region NumPad

        public static void ShowNumPad(Control target)
        {
            ShellPage.Current.ShowNumPad(target);
        }

        #endregion
    }
}
