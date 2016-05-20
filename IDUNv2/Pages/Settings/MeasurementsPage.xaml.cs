
using System;
using IDUNv2.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using IDUNv2.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages.Settings
{

    public sealed partial class MeasurementsPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        #region Keyboard
        private object _host;

        public Control TargetBox { get; private set; }

        public void SetTarget(TextBox control)
        {
            TargetBox = control;
        }

        public void RegisterTarget(TextBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }
        public void RegisterTarget(PasswordBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }

        public void RegisterHost(object host)
        {
            if (host != null)
            {
                _host = host;
            }
        }

        public object GetHost()
        {
            return _host;
        }


        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;
            if (t.FocusState == FocusState.Pointer)
            {
                //this.IsEnabled = true;
                //turn on the lights
            }
        }
        #endregion

        public MeasurementListSettingsVM viewModel = new MeasurementListSettingsVM();

        public object Setting { get; private set; }

        public MeasurementsPage()
        {
            this.InitializeComponent();

            this.Loaded += MeasurementsPage_Loaded;

            SetTarget(ValueTB);

            EnableCheck.Checked += EnableCheck_Checked;
            EnableCheck.Unchecked += EnableCheck_Checked;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (viewModel.CurrentMeasurements.Setting.Threshold.Count == 0)
                ReportSectionPanel.Visibility = Visibility.Collapsed;
            else
                ReportSectionPanel.Visibility = Visibility.Visible;
        }

        private void EnableCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (EnableCheck.IsChecked == true)
                ConfigContent.Visibility = Visibility.Visible;
            else
                ConfigContent.Visibility = Visibility.Collapsed;
        }

        private async void MeasurementsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadMCListFromLocal();
            this.DataContext = viewModel;

            if (EnableCheck.IsChecked == true)
                ConfigContent.Visibility = Visibility.Visible;
            else
                ConfigContent.Visibility = Visibility.Collapsed;

            if (viewModel.CurrentMeasurements.Setting.Threshold.Count == 0)
                ReportSectionPanel.Visibility = Visibility.Collapsed;
            else
                ReportSectionPanel.Visibility = Visibility.Visible;
        }


        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _listView = (ListView)sender;
            var _measurementItem = ((_listView.SelectedItem) as MeasurementListSettingsItems);

            viewModel.CurrentMeasurements = _measurementItem;

        }

        private void TBGotFoc(object sender, RoutedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
            {

                Keyboard.Visibility = Visibility.Visible;
                ReportSectionPanel.Visibility = Visibility.Collapsed;
        }
    }

        private void TBLostFoc(object sender, RoutedEventArgs e)
        {

            var textB = (TextBox)sender;

            if (textB.Text != "")
            {
                if (textB.Text.ToCharArray().Last() == '.')
                {
                    char[] textarray = textB.Text.ToArray();
                    textarray = textarray.Take(textarray.Count() - 1).ToArray();
                    string s = new string(textarray);
                    textB.Text = s;
                }
                if (textB.Text.ToCharArray().Last() == '-')
                {
                    textB.Text = "";
                }
            }
            int chrnr = 0;
            foreach (char chr in textB.Text)
            {
                if (chr == '-') { ++chrnr; }
            }
            if (chrnr > 1)
                textB.Text = "";

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
            {
                Keyboard.Visibility = Visibility.Collapsed;
                ReportSectionPanel.Visibility = Visibility.Visible;
        }
    }
        private void KeyboardBtnClick(object sender, RoutedEventArgs e)
        {

            var target = (TextBox)TargetBox;
            var btn = (Button)sender;

            if ((btn.Content as string) != "←")
            {
                if (!target.Text.Contains("."))
                {
                    target.Text = target.Text + btn.Content;
                }
                else
                {
                    if (btn.Content.ToString() != ".")
                    {
                        target.Text = target.Text + btn.Content;
                    }
                }
            }
            else
            {
                if (target.Text != null || target.Text != string.Empty)
                {
                    char[] textarray = target.Text.ToArray();
                    textarray = textarray.Take(textarray.Count() - 1).ToArray();
                    string s = new string(textarray);
                    target.Text = s;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var op = (Operator)operatorCB.SelectedItem;
                var tp = (Models.Reports.TemplateModel)TemplateCB.SelectedItem;
                double vl;

                if (double.TryParse(ValueTB.Text, out vl) == true)
                {

                    var find = viewModel.CurrentMeasurements.Setting.Threshold.ToList().FindAll(x => x.Value == vl);

                    if (find.Count == 0)
                    {
                        viewModel.CurrentMeasurements.Setting.Threshold.Insert(0, new ThresholdConfig { Operator = op, Template = tp, Value = vl });
                        WarningAdd.Visibility = Visibility.Collapsed; WarningValues.Visibility = Visibility.Collapsed;
                        viewModel.SaveMCListToLocal();
                    }
                    else
                        WarningAdd.Visibility = Visibility.Visible;
                }
                else
                {
                    ValueTB.Focus(FocusState.Keyboard);
                }
            }
            catch
            {
                WarningValues.Visibility = Visibility.Visible;
            }

            if (viewModel.CurrentMeasurements.Setting.Threshold.Count == 0)
                ReportSectionPanel.Visibility = Visibility.Collapsed;
            else
                ReportSectionPanel.Visibility = Visibility.Visible;
        }

        private void RBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = ReportList.SelectedItem as ThresholdConfig;
            viewModel.CurrentMeasurements.Setting.Threshold.Remove(item);
            if (viewModel.CurrentMeasurements.Setting.Threshold.Count == 0)
                ReportSectionPanel.Visibility = Visibility.Collapsed;
            else
                ReportSectionPanel.Visibility = Visibility.Visible;
            viewModel.SaveMCListToLocal();
        }

        public async void LoadMCListFromLocal()
        {
            try
            {
                StorageFile ConfigFile = await localFolder.GetFileAsync("MeasurementConfiguration.txt");
                string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
                viewModel._measurementConfigurationList = JsonConvert.DeserializeObject<ObservableCollection<MeasurementListSettingsItems>>(ConfigText);
            }
            catch
            {

            }
        }

        private async void JSONClick(object sender, RoutedEventArgs e)
        {
            StorageFile ConfigFile = await localFolder.GetFileAsync("MeasurementConfiguration.txt");
            string ConfigText = await FileIO.ReadTextAsync(ConfigFile);
            //JSONShow.Text = ConfigText;
        }
    }
}
