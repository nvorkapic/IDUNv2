﻿using Addovation.Common.Extensions;
using Addovation.Common.Models;
using IDUNv2.Models;
using IDUNv2.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Xamarin;

namespace IDUNv2
{
    public class CloudClient : Addovation.Cloud.Apps.AddoResources.Client.Portable.CloudClient
    {
        #region Properties

        public override string PlatformName => "iOS";
        public override string PlatformVersion => "9.1";
        public override string AppVersion => "2.0.1";
        public override string DeviceId { get; } = Guid.NewGuid().ToString();
        public override string AppId => "TestClient";

        #endregion

        protected override void OnError(string error, Exception ex)
        {
            //throw ex;
            Debug.WriteLine(ex.Message);
        }
    }

    public static class InsightsHelper
    {
        public static void SetUser(ConnectionInfo connectionInfo, string language = null)
        {
            var userId = string.Format("{0} - {1}", connectionInfo.SystemId, connectionInfo.User.ToLower());

            var extraData = connectionInfo.ToDictionary();
            extraData["Language"] = string.IsNullOrEmpty(language) ? Functions.DefaultCulture : language;

            Insights.Identify(userId, extraData);
        }

        public static void ResetUser(string language = null)
        {
            Insights.Identify(null, new Dictionary<string, string>
            {
                {"Language", string.IsNullOrEmpty(language) ? Functions.DefaultCulture : language}
            });
        }

        public static void WriteException(Exception exception, Dictionary<string, string> extraData)
        {
            Insights.Report(exception, extraData);
        }

        public static void WriteError(string error, Dictionary<string, string> extraData)
        {
            WriteException(new Exception(error), extraData);
        }
    }

    public static class AppData
    {
        public static CloudClient CloudClient { get; set; }
        public static FaultReportService FaultReports { get; set; }

        private static void InitInsights()
        {
            var analyticsKey = Insights.DebugModeKey;
            Insights.Initialize(analyticsKey, false);
            InsightsHelper.ResetUser();
        }

        //private static void InitCredential()
        //{
        //    var vault = new PasswordVault();
        //    try
        //    {
        //        var credList = vault.FindAllByResource("idun");
        //    }
        //    catch (Exception)
        //    {
        //        vault.Add(new PasswordCredential("idun", "alex", "alex"));
        //    }
        //}

        public static void InitCloud()
        {
            try
            {
                InitInsights();
                //InitCredential();
                //var vault = new PasswordVault();
                //var cred = vault.FindAllByResource("idun").Where(c => c.UserName == "alain").Single();
                //cred.RetrievePassword();

                var cloudUrl = CommonDictionary.CloudUrls["testcloud.addovation.com"];
                var connectionInfo = new ConnectionInfo(cloudUrl, "race8.addovation.com", "alex", "alex");

                CloudClient = new CloudClient
                {
                    ConnectionInfo = connectionInfo,
                    SessionManager = new Addovation.Cloud.Apps.AddoResources.Client.Portable.SessionManager()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task InitServices()
        {
            FaultReports = new FaultReportService(CloudClient);
            await FaultReports.InitCaches();
        }
    }
}