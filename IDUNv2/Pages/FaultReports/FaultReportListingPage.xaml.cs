using IDUNv2.DataAccess;
using IDUNv2.ViewModels;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class FaultReportListingPage : Page
    {
        private ReportListViewModel viewModel = new ReportListViewModel();

        public FaultReportListingPage()
        {
            this.InitializeComponent();
            Loaded += ReportListPage_Loaded;
        }

        private async void ReportListPage_Loaded(object sender, RoutedEventArgs e)
        {
            var reports = await DAL.GetFaultReports();
            viewModel.Reports = reports.OrderByDescending(r => r.RegDate).ToList();
            this.DataContext = viewModel;
        }

        private void ListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.FaultReportDetailsPage), viewModel.SelectedReport, new DrillInNavigationTransitionInfo());
        }
    }
}
