using IDUNv2.ViewModels;
using IDUNv2.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MeasurementConfig : Page
    {
        //private TemplatesViewModel TemplviewModel = new TemplatesViewModel();
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


        public MeasurementListSettingsItems OriginalItem;

        public object Setting { get; private set; }

        public MeasurementConfig()
        {
            this.InitializeComponent();

            EnableCheck.Checked += EnableCheck_Checked;
            EnableCheck.Unchecked += EnableCheck_Checked;

            SetTarget(ValueTB);
        }

        private void EnableCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (OriginalItem.Setting.Enabled)
            {
                ConfigContent.Visibility = Visibility.Visible;
            }
            else
            {
                ConfigContent.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var PassedParameterItem = (e.Parameter as MeasurementListSettingsItems);
            this.DataContext = PassedParameterItem;

            OriginalItem = PassedParameterItem;


            if (OriginalItem.Setting.Enabled)
            {
                ConfigContent.Visibility = Visibility.Visible;
            }
            else
            {
                ConfigContent.Visibility = Visibility.Collapsed;
            }

            if (ReportList.Items.Count == 0)
            {
                RBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                RBtn.Visibility = Visibility.Visible;
            }

        }

        private void ComboBox1onLoad(object sender, RoutedEventArgs e)
        {
            var Box = (ComboBox)sender;
            Box.SelectedIndex = 0;
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            

            var op = (Operator)operatorCB.SelectedItem;
            var tp = (Template)TemplateCB.SelectedItem;
            double vl;


                if (double.TryParse(ValueTB.Text, out vl) == true)
                {
                    var find = OriginalItem.Setting.Threshold.ToList().FindAll(x => x.Value == vl);

                    if (find.Count == 0)
                    {
                        OriginalItem.Setting.Threshold.Insert(0, new Thresholds { Operator = op, Template = tp, Value = vl });
                        WarningAdd.Visibility = Visibility.Collapsed;
                    }
                    else
                        WarningAdd.Visibility = Visibility.Visible;
                }
                else
                {
                    ValueTB.Focus(FocusState.Keyboard);
                }
   



        }
        private void RemoveButton(object sender, RoutedEventArgs e)
        {
            var item = ReportList.SelectedItem as Thresholds;
            OriginalItem.Setting.Threshold.Remove(item);
        }

        private void ReportList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (ReportList.Items.Count == 0)
            {
                RBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                RBtn.Visibility = Visibility.Visible;
            }
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
                if (textB.Text.ToCharArray().Last() == '.')
                {
                    char[] textarray = textB.Text.ToArray();
                    textarray = textarray.Take(textarray.Count() - 1).ToArray();
                    string s = new string(textarray);
                    textB.Text = s;
                }


                
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
            {

                Keyboard.Visibility = Visibility.Collapsed;
                ReportSectionPanel.Visibility = Visibility.Visible;
            }
        }

        private void TemplateCB_Loaded(object sender, RoutedEventArgs e)
        {
            var Box = (ComboBox)sender;
            Box.SelectedIndex = 0;
        }

        private void KeyboardBtnClick(object sender, RoutedEventArgs e)
        {
        
            var target = (TextBox)TargetBox;
            var btn = (Button)sender;
            
            if ((btn.Content as string) != "Back")
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
    }
}
