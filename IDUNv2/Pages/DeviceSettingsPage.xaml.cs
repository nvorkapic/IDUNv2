using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class DeviceSettingsPage : Page
    {
        #region Fields

        private DeviceSettingsViewModel viewModel = new DeviceSettingsViewModel(DAL.MachineAccess);
        private CmdBarItem[] generalCmdBar;
        private CmdBarItem[] machinesCmdBar;

        public static ShellPage Current;

        #endregion

        #region CmdBar Actions

        private async void SaveDeviceSettings(object param)
        {
            ShellPage.SetSpinner(LoadingState.Loading);
            var status = await DAL.ConnectToCloud();
            viewModel.ConnectionStatus = !status;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2500);
            timer.Tick += Timer_Tick;
            if (status)
            {
                viewModel.AuthorisationMessage = "Log in Successful!";
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "Log In Successful!",
                    "You have been connected to IFS Clouds Service!");

                ShellPage.Current.EnableFullNavList();
            }
            else
            {
                viewModel.AuthorisationMessage = "Authorization Failed. Please Enter Valid details or check your Internet Connection!";
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Warning,
                    "Authorization Failed!",
                    "Please enter valid IFS Cloud Log In details or check your Internet Connection!\nCloud Services are not available.");

                if (viewModel.IsValidated)
                    ShellPage.Current.EnableFullNavList();
            }
            timer.Start();
            viewModel.IsInternet();
            ShellPage.SetSpinner(LoadingState.Finished);
        }

        private void CreateMachine(object param)
        {
            viewModel.CreateMachine();
        }

        private async void SaveMachine(object param)
        {
            await viewModel.SaveMachine();
        }

        private async void DeleteMachine(object param)
        {
            await viewModel.DeleteMachine();
        }

        #endregion

        #region Constructors

        public DeviceSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;

            generalCmdBar = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Save, "Save", SaveDeviceSettings)
            };
            machinesCmdBar = new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Add, "Create", CreateMachine),
                new CmdBarItem(Symbol.Save, "Save", SaveMachine),
                new CmdBarItem(Symbol.Delete, "Delete", DeleteMachine)
            };
        }

        #endregion

        #region Event Handlers

        private void Timer_Tick(object sender, object e)
        {
            viewModel.AuthorisationMessage = "";
            var timer = (DispatcherTimer)sender;
            timer.Stop();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as PasswordBox);
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (viewModel.ObjectID.Length == 0 ||
                viewModel.SystemID.Length == 0 ||
                viewModel.Username.Length == 0 ||
                viewModel.Password.Length <= 3 ||
                viewModel.URL.Length == 0)
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Error,
                    "Device Settings Error",
                    "Please check that all information is entered correctly and try again!");
            }
            else
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "Device Settings Saved",
                    "Device settings have been saved locally.");
            }
        }

        private void Pivot_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if ((string)args.Item.Header == "General")
            {
                DAL.SetCmdBarItems(generalCmdBar);
            }
            else if ((string)args.Item.Header == "Machines")
            {
                DAL.SetCmdBarItems(machinesCmdBar);
            }
            else
            {
                DAL.SetCmdBarItems(null);
            }
        }
        #endregion

        #region Navigation
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel.IsInternet();
            //viewModel.InternetConnectionStatus = false;

            bool status;
            try
            {
                status = await DAL.ConnectToCloud();
            }
            catch
            {
                status = false;
            }

            viewModel.ConnectionStatus = !status;
            await viewModel.InitAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }
        #endregion

        private async void WiFi_StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DAL.SetCmdBarItems(null);
            await viewModel.WiFiAdapterCheck();
        }

        private void ScanForWiFi(object sender, RoutedEventArgs e)
        {
            viewModel.ScanNetwork();          
        }
    }
}
