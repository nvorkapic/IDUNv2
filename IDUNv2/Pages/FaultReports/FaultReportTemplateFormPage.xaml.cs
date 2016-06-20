using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class FaultReportTemplateFormPage : Page
    {
        #region Properties

        private FaultReportTemplateFormViewModel viewModel;

        #endregion

        #region Constructors

        public FaultReportTemplateFormPage()
        {
            viewModel = new FaultReportTemplateFormViewModel(DAL.FaultReportAccess);

            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += Templates_Loaded;
        }

        #endregion

        #region Event Handlers

        private async void Templates_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.InitAsync();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(viewModel.CmdBarItems);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(null);

            int dirtyTemplatesNr = 0;
            if (viewModel.Templates == null)
                return;
            foreach (var item in viewModel.Templates)
            {
                if (item.Dirty)
                    ++dirtyTemplatesNr;
            }
            if (dirtyTemplatesNr >= 1)
            {
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Error,
                    "Unsaved Templates",
                    "Changed templates data and/or new templates were present and not saved. All changes and newly generated templates were discarded!");
            }
        }
    }
}
