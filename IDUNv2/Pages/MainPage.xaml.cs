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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IDUNv2.Pages
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel viewModel = new MainViewModel();

        public object MainMenuList { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            viewModel.SubMenuList = viewModel.subMenus[0];
            ContentFrame.Navigate(viewModel.SubMenuList.FirstOrDefault().PageType);
        }

        private void SubMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListBox;
            var item = lb.SelectedItem as SubMenuItem;
            if (item != null)
                ContentFrame.Navigate(item.PageType);
        }

        private void MainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            var item = lv.SelectedItem as MainMenuItem;
            viewModel.SubMenuList = viewModel.subMenus[lv.SelectedIndex];
            ContentFrame.Navigate(item.PageType);

            Header.Text = item.Label;
        }


    }
}
