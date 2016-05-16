using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SubMenuItem
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
        public string Icon { get; set; }
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
                Icon = "\xE80F",
                //Icon = "/Assets/Home.png",
                SubMenu = new List<SubMenuItem>
                {
                    new SubMenuItem { Title = "Index", PageType = typeof(Pages.Home.IndexPage), Icon = "/Assets/index.png" }
                }
            },
            new MainMenuItem
            {
                Label = "Measurements",
                Icon = "\xE90F",
                //Icon = "/Assets/Ruler.png",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem { Title = "Usage", PageType = typeof(Pages.Measurements.UsagePage),Icon = "/Assets/Usage.png" },
                    new SubMenuItem { Title = "Temperature", PageType = typeof(Pages.Measurements.xMeasurementsPage), Icon = "/Assets/Thermometer.png" },
                    new SubMenuItem { Title = "Pressure", PageType = typeof(Pages.Measurements.xMeasurementsPage), Icon = "/Assets/Pressure.png" },
                    new SubMenuItem {Title = "Humidity", PageType = typeof(Pages.Measurements.xMeasurementsPage), Icon = "/Assets/Humidity.png" },
                    new SubMenuItem {Title = "Accelerometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage), Icon = "/Assets/Accelerometer.png" },
                    new SubMenuItem {Title = "Magnetometer", PageType = typeof(Pages.Measurements.xyzMeasurementsPage),Icon = "/Assets/Magnet.png" },
                    new SubMenuItem {Title = "Gyroscope", PageType = typeof(Pages.Measurements.xyzMeasurementsPage), Icon = "/Assets/Gyroscope.png" }
                }
            },
            new MainMenuItem
            {
                Label = "Reports",
                Icon = "\xE8A5",
                //Icon = "/Assets/Report.png",
                SubMenu =  new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Index", PageType=typeof(Pages.AdditionalApps.Index),Icon = "/Assets/index.png" },
                    new SubMenuItem {Title="LED Control", PageType=typeof(Pages.AdditionalApps.LEDControlPage), Icon = "/Assets/LED.png" },
                    new SubMenuItem {Title="Speech Synthesis", PageType=typeof(Pages.AdditionalApps.SpeechSynthesisPage), Icon = "/Assets/microphone.png" }
                }
            },
            new MainMenuItem
            {
                Label = "Apps",
                Icon = "\xE71D",
                //Icon = "/Assets/Plus.png",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Index", PageType=typeof(Pages.AdditionalApps.Index),Icon = "/Assets/Home.png" },
                    new SubMenuItem {Title="LED Control", PageType=typeof(Pages.AdditionalApps.LEDControlPage), Icon = "/Assets/LED.png" },
                    new SubMenuItem {Title="Speech Synthesis", PageType=typeof(Pages.AdditionalApps.SpeechSynthesisPage), Icon = "/Assets/microphone.png" }
                }
            },
            new MainMenuItem
            {
                Label = "Settings",
                Icon = "\xE713",
                //Icon = "/Assets/Tools.png",
                SubMenu =  new List<SubMenuItem>()
                {
                    new SubMenuItem {Title="Measurement Settings", PageType=typeof(Pages.Settings.MeasurementsPage),Icon = "/Assets/Ruler.png" },
                    new SubMenuItem {Title="Server Settings", PageType=typeof(Pages.Settings.ServerSettingsPage), Icon = "/Assets/Server.png" }
                }
            },
            new MainMenuItem
            {
                Label = "About",
                Icon = "\xE77B",
                //Icon = "/Assets/Information.png",
                SubMenu = new List<SubMenuItem>()
                {
                    new SubMenuItem { Title = "About", PageType=typeof(Pages.About.Index), Icon = "/Assets/index.png", }
                }
            }
        };

        public List<MainMenuItem> MainMenu { get { return _mainMenu; } }

        private string _mainTitle;
        public string MainTitle { get { return _mainTitle; } set { _mainTitle = value; Notify(); } }

        private string _mainIcon;
        public string MainIcon { get { return _mainIcon; } set { _mainIcon = value; Notify(); } }

        private string _subTitle;
        public string SubTitle { get { return _subTitle; } set { _subTitle = value; Notify(); } }

        private List<SubMenuItem> _subMenu;
        public List<SubMenuItem> SubMenu { get { return _subMenu; } set { _subMenu = value; Notify(); } }
    }
}
