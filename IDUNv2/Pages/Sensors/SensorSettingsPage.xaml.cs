using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class SensorSettingsPage : Page
    {
        #region Fields

        private SensorSettingsViewModel viewModel;

        #endregion

        #region Constructors

        public SensorSettingsPage()
        {
            viewModel = new SensorSettingsViewModel(DAL.SensorTriggerAccess, DAL.FaultReportAccess);
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        #endregion

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel.Sensor = e.Parameter as Sensor;
            await viewModel.InitAsync();
            DAL.PushNavLink(new NavLinkItem("Settings", GetType(), e.Parameter));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }

        #region Event Handlers

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowNumPad(sender as TextBox);
            TriggeerList.Visibility = Visibility.Collapsed;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowNumPad(null);
            TriggeerList.Visibility = Visibility.Visible;
        }

        private void TextBoxUnit_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);
        }

        private void TextBoxUnit_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        private void Templates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            var item = (FaultReportTemplate)cb.SelectedItem;
            if (item != null)
            {
                viewModel.SetSelectedTriggerFromTemplate(item);
            }
        }

        private void Triggers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.UpdateSelectedTemplate();
        }

        private void Pivot_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if (args.Item.Header.ToString() == "General")
            {
                DAL.SetCmdBarItems(viewModel.GeneralCmdBarItems);
            }
            else
            {
                DAL.SetCmdBarItems(viewModel.TriggerCmdBarItems);
            }
        }

        #endregion
    }
}
