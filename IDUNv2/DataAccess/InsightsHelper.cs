using Addovation.Common.Extensions;
using Addovation.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin;

namespace IDUNv2.DataAccess
{
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

        public static void Init()
        {
            var analyticsKey = Insights.DebugModeKey;
            Insights.Initialize(analyticsKey, false);
            InsightsHelper.ResetUser();
        }
    }
}
