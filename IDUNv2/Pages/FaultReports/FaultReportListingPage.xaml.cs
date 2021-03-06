﻿using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
            var reports = await faultReportAccess.GetFaultReports(DeviceSettings.ObjectID);
            ShellPage.SetSpinner(LoadingState.Finished);
            Reports = reports.OrderByDescending(r => r.RegDate).ToList();
            this.DataContext = this;
        }

        private void ListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lb = sender as ListBox;
            var report = lb.SelectedItem as FaultReport;
            if (report != null)
            {
                Frame.Navigate(typeof(FaultReportDetailsPage), report, new DrillInNavigationTransitionInfo());
            }
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);
        }
        #endregion
    }
}
