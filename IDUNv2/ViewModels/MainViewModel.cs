using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SubMenuItem
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
    }

    public class MainMenuItem
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public Type PageType { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        public List<List<SubMenuItem>> subMenus = new List<List<SubMenuItem>>()
            {
                new List<SubMenuItem>()
                {
                    //Home
                    //Index 0
                },
                new List<SubMenuItem>()
                {
                    //Measurements
                    //Index 1
                    new SubMenuItem { Title = "Index", PageType = typeof(Pages.Measurements.UsagePage) },
                    new SubMenuItem { Title = "Usage", PageType = typeof(Pages.Measurements.UsagePage) },
                    new SubMenuItem { Title = "Temperature", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem { Title = "Pressure", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem {Title = "Humidity", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem {Title = "Accelerometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) },
                    new SubMenuItem {Title = "Magnetometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) },
                    new SubMenuItem {Title = "Gyroscope", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) }
                },
                new List<SubMenuItem>()
                {
                    //Additional Applicationa
                    //Index 2
                    new SubMenuItem {Title="Index", PageType=typeof(Pages.AdditionalApps.Index) },
                    new SubMenuItem {Title="LED Control", PageType=typeof(Pages.AdditionalApps.LEDControlPage) },
                    new SubMenuItem {Title="Speech Synthesis", PageType=typeof(Pages.AdditionalApps.SpeechSynthesisPage) }
                },
                new List<SubMenuItem>()
                {
                    //Reports
                    //Index 3
                    new SubMenuItem {Title="Overview", PageType=typeof(Pages.Reports.Index) },
                    new SubMenuItem {Title="Report Templates", PageType=typeof(Pages.Reports.TemplatesPage) }
                },
                new List<SubMenuItem>()
                {
                    //Settings
                    //Index 4
                    new SubMenuItem {Title="Measurement Settings", PageType=typeof(Pages.Settings.MeasurementsPage) },
                    new SubMenuItem {Title="Server Settings", PageType=typeof(Pages.Settings.ServerSettingsPage) }
                },
                new List<SubMenuItem>()
                {
                    //About
                    //Index 5
                }

        };

        private List<SubMenuItem> _subMenulist;
        public List<SubMenuItem> SubMenuList { get { return _subMenulist; } set { _subMenulist = value; Notify(); } }


        public List<MainMenuItem> mainMenu = new List<MainMenuItem>()
        {
            new MainMenuItem {Label = "Home", Icon = "Home", PageType=typeof(Pages.Home.IndexPage) },
            new MainMenuItem {Label="Measurements", Icon = "", PageType=typeof(Pages.Measurements.Index) },
            new MainMenuItem {Label="Additional Applications", Icon="", PageType=typeof(Pages.AdditionalApps.Index) },
            new MainMenuItem {Label="Reports", Icon="", PageType=typeof(Pages.Reports.Index) },
            new MainMenuItem {Label = "Settings", Icon = "Setting", PageType=typeof(Pages.Settings.MeasurementsPage) },
            new MainMenuItem {Label ="About", Icon="", PageType=typeof(Pages.About.Index) }
        };

        public List<MainMenuItem> MainMenuList { get { return mainMenu; } set { mainMenu = value; } }
    }
}
