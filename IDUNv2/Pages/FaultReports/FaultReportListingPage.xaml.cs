using Addovation.Cloud.Apps.AddoResources.Client.Portable;
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
        #region Fields

        private IFaultReportAccess faultReportAccess = DAL.FaultReportAccess;

        #endregion

        #region Properties

        public List<FaultReport> Reports { get; private set; } = new List<FaultReport>();

        #endregion

        #region Constructors

        public FaultReportListingPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            Loaded += ReportListPage_Loaded;
        }

        #endregion

        #region Event Handlers

        private async void ReportListPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

            ShellPage.SetSpinner(LoadingState.Loading);
            await faultReportAccess.FillCaches();
            ShellPage.SetSpinner(LoadingState.Finished);

            var reports = await faultReportAccess.GetFaultReports();
            Reports = reports.OrderByDescending(r => r.RegDate).ToList();
            this.DataContext = this;
        }

        private void ListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lb = sender as ListBox;
            Frame.Navigate(typeof(Pages.FaultReportDetailsPage), lb.SelectedItem, new DrillInNavigationTransitionInfo());
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }
    }
}
