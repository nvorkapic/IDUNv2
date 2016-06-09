using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    public static class DeviceSettings
    {
        private static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static string ObjectID
        {
            get { return localSettings.Values["ObjectID"] as string; }
            set { localSettings.Values["ObjectID"] = value; }
        }

        public static string SystemID
        {
            get { return localSettings.Values["SystemID"] as string; }
            set { localSettings.Values["SystemID"] = value; }
        }

        public static string URL
        {
            get { return localSettings.Values["URL"] as string; }
            set { localSettings.Values["URL"] = value; }
        }

        public static string Username
        {
            get { return localSettings.Values["Username"] as string; }
            set { localSettings.Values["Username"] = value; }
        }

        public static string Password
        {
            get { return localSettings.Values["Password"] as string; }
            set { localSettings.Values["Password"] = value; }
        }

        public static bool HasSettings()
        {
            return SystemID != null && URL != null && Username != null && Password != null;
        }
    }
}
