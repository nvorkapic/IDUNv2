using IDUNv2.Models;
using IDUNv2.Services;
using IDUNv2.ViewModels;
using SQLite;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TriggerList : Page
    {
        SensorService ss = new SensorService();
        
        public SensorTriggerViewModel viewModel = new SensorTriggerViewModel();
        public SensorTrigger CurrentTrigger = new SensorTrigger();
        
        public TriggerList()
        {
            this.InitializeComponent();
            this.Loaded += TriggerList_Loaded;
        }

        private void TriggerList_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = viewModel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           
            if (CurrentTrigger != null)
            {
                
                string WarningHeader = "Report Trigger Removed from the List";
                string WarningContent = "Report Trigger: " + CurrentTrigger.SensorId.ToString() + " that fires when value goes" + CurrentTrigger.Comparer.ToString() + " " + CurrentTrigger.Value.ToString() + " and uses " + CurrentTrigger.TemplateId.ToString() + " TemplateId has been removed.";
                await ss.DeleteTrigger(CurrentTrigger);
                TriggerListView.ItemsSource = viewModel.SensorTriggerList;

                var dialog = new ContentDialog { Title = WarningHeader, Content = WarningContent, HorizontalAlignment=HorizontalAlignment.Center, VerticalAlignment=VerticalAlignment.Center, PrimaryButtonText="OK"};
   
                var showdialog = await dialog.ShowAsync();

                
            }
        }

        private void ListSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            CurrentTrigger = list.SelectedItem as SensorTrigger;
        }

    }

}
