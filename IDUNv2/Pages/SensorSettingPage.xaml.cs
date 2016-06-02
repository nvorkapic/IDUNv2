
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
using IDUNv2.Services;
using SQLite.Net;
using Windows.UI;
using System.Text.RegularExpressions;

namespace IDUNv2.Pages
{
    public sealed partial class SensorSettingPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private static SQLiteConnection db = new SQLiteConnection(AppData.SqlitePlatform, AppData.DbPath);

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

        public SensorTriggerViewModel viewModel = new SensorTriggerViewModel();

        public SensorSettingPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            ElementCount();
            SetTarget(ValueTB);
        }


        public float value = 0;
        private async void AddTrigger(object sender, RoutedEventArgs e)
        {
 
            if (viewModel.SensorTriggerList.Where(x => x.SensorId == viewModel.CurrentTrigger.SensorId).
                Where(x => x.TemplateId == viewModel.CurrentTrigger.TemplateId).
                Where(x => x.Comparer == viewModel.CurrentTrigger.Comparer).
                Where(x => x.Value == viewModel.CurrentTrigger.Value).Count() == 0)
            {
                viewModel.AddTrigger();
                ElementCount();

                await AppData.InitCloud();
                var templates = AppData.Reports.GetTemplates().Result;
                var template = templates.Where(x => x.Id == viewModel.CurrentTrigger.TemplateId).FirstOrDefault().Name;

                var Header = "Trigger Added";
                var Content = "Report Trigger: " + viewModel.CurrentTrigger.SensorId.ToString() + " that fires when value goes " + viewModel.CurrentTrigger.Comparer.ToString() + " " + viewModel.CurrentTrigger.Value.ToString() + " and uses Template " + template + ", has been Added.";
                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, Header, Content);
                //var dialog = new ContentDialog { Title = Header, Content = Content, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RequestedTheme = ElementTheme.Dark , PrimaryButtonText = "OK" };
                //var showdialog = await dialog.ShowAsync();
            }
        }

        public void ElementCount()
        {
            TriggerCount.Text = viewModel.SensorTriggerList.Count().ToString();
            UsageNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Usage).Count().ToString();
            TemperatureNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Temperature).Count().ToString();
            PressureNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Pressure).Count().ToString();
            HumidityNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Humidity).Count().ToString();
            AccelerometerNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Accelerometer).Count().ToString();
            MagnetometerNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Magnetometer).Count().ToString();
            GyroscopeNumber.Text = viewModel.SensorTriggerList.Where(x => x.SensorId == SensorType.Gyroscope).Count().ToString();
        }

        private void ViewTriggerList(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TriggerList));
        }

        private void TBOnFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.Visibility = Visibility.Visible;
            var tb = (TextBox)sender;
            tb.Text = "";
        }

        private void TBLostFocus(object sender, RoutedEventArgs e)
        {
           Keyboard.Visibility = Visibility.Collapsed;

           var textB = (TextBox)sender;
          
           if (textB.Text != "")
                if (textB.Text.ToCharArray().Last() == '.')
                {
                    char[] textarray = textB.Text.ToArray();
                    textarray = textarray.Take(textarray.Count() - 1).ToArray();
                    string s = new string(textarray);
                    textB.Text = s;
                }
        }

        private void templateOnLoad(object sender, RoutedEventArgs e)
        {
            var lv = (ComboBox)sender;
            lv.SelectedItem = lv.Items.FirstOrDefault();
        }

        private void AboveChecked(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentTrigger.Comparer = SensorTriggerComparer.Above;
        }

        private void BelowChecked(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentTrigger.Comparer = SensorTriggerComparer.Below;
        }

        private void KeyboardBtnClick(object sender, RoutedEventArgs e)
        {
        
            var target = (TextBox)TargetBox;
            var btn = (Button)sender;

            target.Text = target.Text + btn.Content;
     
        }
   
        private void KeyboardBackBtnClick(object sender, RoutedEventArgs e)
        {
            var target = (TextBox)TargetBox;
            var btn = (Button)sender;

            if (target.Text != null || target.Text != string.Empty)
            {
                char[] textarray = target.Text.ToArray();
                textarray = textarray.Take(textarray.Count() - 1).ToArray();
                string s = new string(textarray);
                target.Text = s;

            }
        }
        private void KeyboardMinusBtnClick(object sender, RoutedEventArgs e)
        {
            var target = (TextBox)TargetBox;
            var btn = (Button)sender;

            if (target.Text != null || target.Text != string.Empty)
            {
                if (target.Text.ElementAt(0) == '-')
                {
                    string[] parts = Regex.Split(target.Text, "-").Skip(1).ToArray();
                    target.Text = string.Join("", parts);
                }
                else
                    target.Text = "-" + target.Text;
            }
        }
        private void KeyboardDecimalClick(object sender, RoutedEventArgs e)
        {
            var target = (TextBox)TargetBox;
            var btn = (Button)sender;
            
            if (!target.Text.Contains("."))
            {
                target.Text = target.Text + ".";
            }
        }

}
}

