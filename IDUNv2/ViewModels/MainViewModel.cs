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

    public class MainMenuItem : BaseViewModel
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public List<SubMenuItem> SubMenu { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        private List<MainMenuItem> _mainMenu = new List<MainMenuItem>
        {
            new MainMenuItem
            {
                Label = "Home",
                SubMenu = new List<SubMenuItem>
                {
                    new SubMenuItem { Title = "Home", PageType = typeof(Pages.Home.IndexPage) }
                }
            },
            new MainMenuItem
            {
                Label = "Measurements",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem { Title = "Index", PageType = typeof(Pages.Measurements.Index) },
                    new SubMenuItem { Title = "Usage", PageType = typeof(Pages.Measurements.UsagePage) },
                    new SubMenuItem { Title = "Temperature", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem { Title = "Pressure", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem {Title = "Humidity", PageType = typeof(Pages.Measurements.xMeasurementsPage) },
                    new SubMenuItem {Title = "Accelerometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) },
                    new SubMenuItem {Title = "Magnetometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) },
                    new SubMenuItem {Title = "Gyroscope", PageType = typeof(Pages.Measurements.xyzMeasurementsPage) }
                }
            },
            new MainMenuItem
            {
                Label = "Apps",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Index", PageType=typeof(Pages.AdditionalApps.Index) },
                    new SubMenuItem {Title="LED Control", PageType=typeof(Pages.AdditionalApps.LEDControlPage) },
                    new SubMenuItem {Title="Speech Synthesis", PageType=typeof(Pages.AdditionalApps.SpeechSynthesisPage) }
                }
            },
            new MainMenuItem
            {
                Label = "Reports",
                SubMenu =  new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Overview", PageType=typeof(Pages.Reports.Index) },
                    new SubMenuItem {Title="Report Templates", PageType=typeof(Pages.Reports.TemplatesPage) }
                }
            },
            new MainMenuItem
            {
                Label = "Settings",
                SubMenu =  new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Measurement Settings", PageType=typeof(Pages.Settings.MeasurementsPage) },
                    new SubMenuItem {Title="Server Settings", PageType=typeof(Pages.Settings.ServerSettingsPage) }
                }
            },
            new MainMenuItem
            {
                Label = "About",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem { Title = "About", PageType=typeof(Pages.About.Index) }
                }
            }
        };

        public List<MainMenuItem> MainMenu { get { return _mainMenu; } }

        private List<SubMenuItem> _subMenu;
        public List<SubMenuItem> SubMenu { get { return _subMenu; } set { _subMenu = value; Notify(); } }
    }
}
