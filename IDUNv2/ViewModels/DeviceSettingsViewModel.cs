using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class DeviceSettingsViewModel : NotifyBase
    {
        public string ObjectID
        {
            get { return DeviceSettings.ObjectID; }
            set { DeviceSettings.ObjectID = value; Notify(); }
        }

        public string SystemID
        {
            get { return DeviceSettings.SystemID; }
            set { DeviceSettings.SystemID = value; Notify(); }
        }

        public string URL
        {
            get { return DeviceSettings.URL; }
            set { DeviceSettings.URL = value; Notify(); }
        }

        public string Username
        {
            get { return DeviceSettings.Username; }
            set { DeviceSettings.Username = value; Notify(); }
        }

        public string Password
        {
            get { return DeviceSettings.Password; }
            set { DeviceSettings.Password = value; Notify(); }
        }

        private string _authorisationMessage;
        public string AuthorisationMessage
        {
            get { return _authorisationMessage; }
            set { _authorisationMessage = value; Notify(); }
        }

        private bool _connectionStatus;
        public bool ConnectionStatus
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; Notify(); }
        }

        public string ConnectionMessage
        {
            get
            {
                if (ConnectionStatus)
                    return "Connected";
                else
                    return "Disconneted";
            }
        }
    }
}
