using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.DataAccess;
using IDUNv2.ViewModels;
using System;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static IDUNv2.DataAccess.DAL;

namespace IDUNv2.Pages
{
    public sealed partial class FaultReportDetailsPage : Page
    {
        FaultReportDetailsViewModel viewModel = new FaultReportDetailsViewModel(DAL.FaultReportAccess);


        public FaultReportDetailsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {          
            base.OnNavigatedTo(e);

            viewModel.Model = e.Parameter as FaultReport;

            this.DataContext = viewModel;

            ShellPage.SetSpinner(LoadingState.Loading);
            await viewModel.InitAsync();
            ShellPage.SetSpinner(LoadingState.Finished);         
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem != null)
            {
                var attachementData = await DAL.cloud.GetAttachmentData(((ListView)sender).SelectedItem as Attachment);
                var filedataText = Encoding.UTF8.GetString(Convert.FromBase64String(attachementData.FileData));
                viewModel.AttachementDataText = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentString>(filedataText);
          
                AttachementTooltip.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AttachementTooltip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ListViewAttachements.SelectedIndex = -1;
        }


    }
}
