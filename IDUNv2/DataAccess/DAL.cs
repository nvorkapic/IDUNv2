using Addovation.Common.Models;
using IDUNv2.Models;
using IDUNv2.Pages;
using IDUNv2.SensorLib;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using IDUNv2.SensorLib.IMU;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;

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
        private static volatile int dialogCount;
        private static readonly string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        private static readonly SQLiteConnection db = new SQLiteConnection(new SQLitePlatformWinRT(), dbPath);

        private static CachingCloudClient cloud;
        private static SensorWatcher sensorWatcher;
        public static ImuSensorWatcher ImuSensorWatcher;

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

            ImuSensorWatcher = new ImuSensorWatcher(dispatcher, 10);

            SensorAccess = new SensorAccess(sensorWatcher);
            SensorTriggerAccess = new SensorTriggerAccess(db);

            CreateCloudClient();

            InstallSensorFaultHandler();
        }

        #region Fault Handlers

        private static async Task ShowDialog(Sensor sensor, SensorFault fault, FaultReport report)
        {
            var dialog = new ContentDialog { Title = "Faulted" };
            dialog.Loaded += async (sender, e) =>
            {
                await Task.Delay(4000);
                dialog.Hide();
            };
            var panel = new StackPanel();
            var text = new TextBlock();

            text.Text = $"Sensor '{sensor.Id}' faulted with type: {fault.Type}\n";

            if (report == null)
            {
                dialog.Title += " (ERROR)";
                text.Text += "but failed to send a fault report, check the cloud connection";
            }
            else
            {
                text.Text = $"Report generated with Work Order: {report.WoNo}";
            }

            panel.Children.Add(text);
            dialog.Content = panel;
            dialog.SecondaryButtonText = "Close";
            dialog.IsSecondaryButtonEnabled = true;

            if (report != null)
            {
                dialog.PrimaryButtonText = "View Report";
                dialog.IsPrimaryButtonEnabled = true;
                dialog.PrimaryButtonCommand = new ActionCommand<object>(o =>
                {
                    ShellPage.Current.ContentNavigate(typeof(FaultReportDetailsPage), report);
                });
            }

            Interlocked.Increment(ref dialogCount);
            await dialog.ShowAsync();
        }

        private static async Task<FaultReport> SendFaultReport(Sensor sensor, SensorFault fault, DateTime timestamp)
        {
            if (fault.Type == SensorFaultType.FromTrigger)
            {
                var trigger = await SensorTriggerAccess.FindSensorTrigger(fault.Id);
                if (trigger == null)
                    return null;

                var template = await FaultReportAccess.FindFaultReportTemplate(trigger.TemplateId);
                if (template == null)
                    return null;

                try
                {
                    var machine = Machine.Machines[DeviceSettings.ObjectID];
                    var report = new FaultReport
                    {
                        MchCode = machine.MchCode,
                        MchCodeContract = machine.MchCodeContract,
                        ErrDescr = template.Directive,
                        ErrDescrLo = template.FaultDescr,
                        ErrDiscoverCode = template.DiscCode,
                        ErrSymptom = template.SymptCode,
                        PriorityId = template.PrioCode,
                        OrgCode = machine.OrgCode
                    };

                    return await FaultReportAccess.SetFaultReport(report);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        private static void InstallSensorFaultHandler()
        {
            SensorAccess.Faulted += async (sensor, fault, timestamp) =>
            {
                var report = await SendFaultReport(sensor, fault, timestamp);

                if (dialogCount == 0)
                {
                    await ShowDialog(sensor, fault, report).ContinueWith(task =>
                    {
                        Interlocked.Decrement(ref dialogCount);
                    });
                }
                string shortDescription = "Sensor Triggered";
                string longDescription = "Sensor has entered Triggered State!\n\nSensor ID: " + sensor.Id + "\nFaulted State: " + sensor.FaultState + "\nDevice State: " + sensor.DeviceState + "\nSensor Value: " + sensor.Value + "\nSensor Danger High Value: " + sensor.DangerHi + "\nSensor Danger Low Value: " + sensor.DangerLo + "\nSensor Maximum Value: " + sensor.RangeMax + "\nSensor Minimum Value: " + sensor.RangeMin + "\nFault ID: " + fault.Id + "\nFault Type: " + fault.Type;

                ShellPage.Current.AddNotificatoin(NotificationType.Warning, shortDescription, longDescription);

                DocumentString document = new DocumentString();
                document.shortDescription = shortDescription;
                document.longDescription = longDescription;

                try
                {
                    string json = JsonConvert.SerializeObject(document);
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder TriggerReportsFolder = await localFolder.CreateFolderAsync("TriggerReports", CreationCollisionOption.OpenIfExists);
                    var TriggerReportFile = await TriggerReportsFolder.CreateFileAsync("TriggerReport", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(TriggerReportFile, json);
                }
                catch { }
            };
        }

        public class DocumentString
        {
            public string shortDescription { get; set; }
            public string longDescription { get; set; }
            public string Date { get { return DateTime.Now.ToString(); } }
            public string DeviceID { get { return DeviceSettings.ObjectID; } }
            public string SystemID { get { return DeviceSettings.SystemID; } }
        }

        #endregion

        #region Cloud

        private static void CreateCloudClient()
        {
            try
            {
                string url = DeviceSettings.URL;
                string systemid = DeviceSettings.SystemID;
                string username = DeviceSettings.Username;
                string password = DeviceSettings.Password;

                string cloudUrl = "";
                try
                {
                    cloudUrl = CommonDictionary.CloudUrls[url];
                }
                catch (KeyNotFoundException)
                {
                    cloudUrl = url;
                }
                var connectionInfo = new ConnectionInfo(cloudUrl, systemid, username, password);

                cloud = new CachingCloudClient
                {
                    ConnectionInfo = connectionInfo,
                    SessionManager = new Addovation.Cloud.Apps.AddoResources.Client.Portable.SessionManager()
                };

                InsightsHelper.Init();

                //FaultReportAccess = new FaultReportAccess(cloud, db);
                FaultReportAccess = new MockFaultReportAccess();
            }
            catch
            {

            }
        }

        public static Task<bool> ConnectToCloud()
        {
            CreateCloudClient();
            if (cloud != null)
                return cloud.Authenticate();
            return Task.FromResult(false);
        }

        public static async Task FillCaches()
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            if (cloud != null)
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
