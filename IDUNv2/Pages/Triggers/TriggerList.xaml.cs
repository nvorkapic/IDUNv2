using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{

    public sealed partial class TriggerList : Page
    {
        public SensorTriggerSettingViewModel viewModel = new SensorTriggerSettingViewModel();
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
                var templates = await DAL.GetFaultReportTemplates();
                var template = templates.Where(x => x.Id == CurrentTrigger.TemplateId).FirstOrDefault().Name;

                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "Trigger Removed", "Report Trigger: " + "<SENSOR>" + " that fires when value goes " + CurrentTrigger.Comparer.ToString() + " " + CurrentTrigger.Value.ToString() + " and uses Template " + template + ", has been Removed.");
                await DAL.DeleteSensorTrigger(CurrentTrigger);
                viewModel.SensorTriggerList.Remove(CurrentTrigger);
                //Doesn't reload list anymore
                TriggerListView.ItemsSource = viewModel.SensorTriggerList;
                
            }
        }

        private void ListSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            CurrentTrigger = list.SelectedItem as SensorTrigger;
        }

        private async void DoubleTapList(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                var ListItem = ((ListView)sender).SelectedItem as SensorTrigger;
                var TemplateID = (((ListView)sender).SelectedItem as SensorTrigger).TemplateId;
                var Templates = await DAL.GetFaultReportTemplates();

                var SelectedTemplate = Templates.Where(x => x.Id == TemplateID).FirstOrDefault();

                await DAL.FillCaches();
                var Sympt = DAL.GetWorkOrderSymptom(SelectedTemplate.SymptCode).Description;
                var Prio = DAL.GetWorkOrderPiority(SelectedTemplate.PrioCode).Description;
                var Disc = DAL.GetWorkOrderDiscovery(SelectedTemplate.DiscCode).Description;
                
                
                var contentString = "Sensor: " + "<SENSOR>" + " " + ListItem.Comparer + " " + ListItem.Value + "\nTemplate\n Name: " + SelectedTemplate.Name + "\n Symptom: " + Sympt + "\n Priority: " + Prio + "\n Discovery: " + Disc;
                
                SelectedTrigger selectedTrigger = new SelectedTrigger { Sensor = "<SENSOR>", Comparer = ListItem.Comparer.ToString(), Value = ListItem.Value.ToString(), TemplateName = SelectedTemplate.Name, Symptom = Sympt, Discovery = Disc, Priority = Prio };

                this.Frame.Navigate(typeof(SelectedTriggerPage), selectedTrigger);
                //var dialog = new ContentDialog { Title = "Selected Trigger", Content = contentString, PrimaryButtonText = "OK", RequestedTheme = ElementTheme.Dark };
                //var showdialog = await dialog.ShowAsync();
            }
            catch
            {

            }
        }
    }
}
