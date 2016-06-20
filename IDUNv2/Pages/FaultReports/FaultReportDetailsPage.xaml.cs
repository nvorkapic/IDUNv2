using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.DataAccess;
using IDUNv2.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class FaultReportDetailsPage : Page
    {
        FaultReportDetailsViewModel viewModel = new FaultReportDetailsViewModel(DAL.FaultReportAccess);

        public FaultReportDetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel.Model = e.Parameter as FaultReport;

            this.DataContext = viewModel;

        }
    }
}
