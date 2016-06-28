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
        #region Notify Fields

        private string _authorisationMessage;
        private bool _connectionStatus;

        #endregion

        #region Notify Properties

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

        public string AuthorisationMessage
        {
            get { return _authorisationMessage; }
            set { _authorisationMessage = value; Notify(); }
        }

        public bool ConnectionStatus
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; Notify(); Notify("ConnectionMessage"); }
        }

        #endregion

        #region Properties

        public string ConnectionMessage
        {
            get
            {
                if (!ConnectionStatus)
                    return "Connected";
                else
                    return "Disconneted";
            }
        }

        public bool IsValidated
        {
            get { return DeviceSettings.HasSettings(); }
        }

        public List<Machine> Machines { get; }

        #endregion

        #region Constructors

        public DeviceSettingsViewModel()
        {
            Machines = Machine.Machines.Values.ToList();
        }

        #endregion
    }
}
