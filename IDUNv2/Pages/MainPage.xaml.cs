using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace IDUNv2.Pages
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel viewModel = new MainViewModel();
        public MeasurementListSettingsVM MEasurementListTest = new MeasurementListSettingsVM();
        public MainPage()
        {
            
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += MainPage_Loaded;

            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var first = viewModel.MainMenu.First();
            viewModel.SelectMainMenu(ContentFrame, first);
        }

        private void MainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            var item = lv.SelectedItem as MainMenuItem;
            viewModel.SelectMainMenu(ContentFrame, item);
        }

        private void SubMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListView;
            var item = lb.SelectedItem as SubMenuItem;
            if (item != null)
            {
                viewModel.SelectSubMenu(ContentFrame, item);
                //HeaderSubImage.Source = new BitmapImage (new Uri("ms-appx://IDUNv2" + item.Icon));
            }
                
            if (lb.SelectedItem == null)
            {
                lb.SelectedIndex = 0;
            }
        }

        private void MainMenuExpand_Click(object sender, RoutedEventArgs e)
        {
            mainMenuSplitView.IsPaneOpen = !mainMenuSplitView.IsPaneOpen;
        }

        private void subLoaded(object sender, RoutedEventArgs e)
        {
            (sender as ListView).SelectedIndex = 0;
        }
    }
}
