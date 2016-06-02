using Addovation.Cloud.Apps.AddoResources.Client.Portable;
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
    public class ReportListViewModel
    {
        public List<FaultReport> Reports { get; set; }
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
    }
}
