using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using System;
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
namespace IDUNv2.Pages
{
    public class ReportListViewModel : NotifyBase
    {
        public List<FaultReport> Reports { get; set; }

        private FaultReport selectedReport;
        public FaultReport SelectedReport
        {
            get { return selectedReport; }
            set { selectedReport = value; Notify(); }
        }
    }

    public sealed partial class ReportListPage : Page
    {
        private ReportListViewModel viewModel = new ReportListViewModel();

        public ReportListPage()
        {
            this.InitializeComponent();
            Loaded += ReportListPage_Loaded;
        }

        private async void ReportListPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AppData.InitServices();
            var reports = await AppData.Reports.GetFaultReports();
            viewModel.Reports = reports.OrderByDescending(r => r.RegDate).ToList();
            this.DataContext = viewModel;
        }

        private void ListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.ReportDetailsPage), viewModel.SelectedReport, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
