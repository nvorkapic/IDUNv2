using IDUNv2.ViewModels;
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
    public sealed partial class MeasurementConfig : Page
    {

        public MeasurementListSettingsItems OriginalItem;

        public object Setting { get; private set; }

        public MeasurementConfig()
        {
            this.InitializeComponent();

            EnableCheck.Checked += EnableCheck_Checked;
            EnableCheck.Unchecked += EnableCheck_Checked;
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

            OriginalItem.Setting.Threshold.Add(new Thresholds { Operator = op , Template = tp, Value = double.Parse(ValueTB.Text) });

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
    }
}
