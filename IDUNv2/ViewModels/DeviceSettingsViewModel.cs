using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace IDUNv2.ViewModels
{
    public class DeviceSettingsViewModel : NotifyBase
    {
        #region Fields

        private IMachineAccess machineAccess;

        #endregion

        #region Notify Fields

        private string _authorisationMessage;
        private bool _connectionStatus;
        private ObservableCollection<MachineViewModel> _machines;
        private MachineViewModel _selectedMachine;
        private bool _internetConnectionStatus;
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

        public bool InternetConnectionStatus
        {
            get { return _internetConnectionStatus; }
            set { _internetConnectionStatus = value; Notify(); Notify("InternetConnectionMessage"); }
        }
        public MachineViewModel SelectedMachine
        {
            get { return _selectedMachine; }
            set { _selectedMachine = value; Notify(); }
        }

        public ObservableCollection<MachineViewModel> Machines
        {
            get { return _machines; }
            set { _machines = value; Notify(); }
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

        public string InternetConnectionMessage
        {
            get
            {
                if (!InternetConnectionStatus)
                    return "No Internet Connection";
                else
                    return "Internet Connection Present";
            }
        }

        public bool IsValidated
        {
            get { return DeviceSettings.HasSettings(); }
        }

        public void IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            InternetConnectionStatus = internet;
        }
        #endregion

        #region Constructors

        public DeviceSettingsViewModel(IMachineAccess machineAccess)
        {
            this.machineAccess = machineAccess;
        }

        #endregion

        public async Task InitAsync()
        {
            var machines = await machineAccess.GetMachines();
            Machines = new ObservableCollection<MachineViewModel>(machines.Select(m => new MachineViewModel(m)));
            Notify("ObjectID");
            SelectedMachine = Machines.FirstOrDefault();
        }


        public void CreateMachine()
        {
            var mch = new MachineViewModel(new Machine { MchCode = "#New Machine" });
            Machines.Add(mch);
            SelectedMachine = Machines.LastOrDefault();
            SelectedMachine.Dirty = true;
        }

        public async Task SaveMachine()
        {
            if (SelectedMachine == null)
                return;
            try
            {
                if (SelectedMachine.IsValidated)
                {
                    SelectedMachine.Model = await machineAccess.SetMachine(SelectedMachine.Model);
                    SelectedMachine.Dirty = false;
                    ShellPage.Current.AddNotificatoin(
                        NotificationType.Information,
                        "Machine Saved",
                        $"Machine ({SelectedMachine.MchCode}) was successfully saved");
                }
                else
                {
                    ShellPage.Current.AddNotificatoin(
                        NotificationType.Error,
                        "Validation Error",
                        "MchCode, MchCodeContract and OrgCode are all required fields");
                }
            }
            catch (Exception ex)
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Error,
                    "Exception",
                    ex.ToString());
            }
        }

        public async Task DeleteMachine()
        {
            if (Machines == null || SelectedMachine == null)
                return;

            try
            {
                bool result = await machineAccess.DeleteMachine(SelectedMachine.Model);
                Machines.Remove(SelectedMachine);
                SelectedMachine = Machines.LastOrDefault();
                if (result && SelectedMachine != null)
                {
                    if (SelectedMachine != null)
                    {
                        ShellPage.Current.AddNotificatoin(
                            NotificationType.Information,
                            "Machine Deleted",
                            $"Successfully deleted machine ({SelectedMachine.MchCode})");
                    }
                }
            }
            catch (Exception ex)
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Error,
                    "Exception",
                    ex.ToString());
            }
        }
    }
}
