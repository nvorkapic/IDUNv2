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
                    new SubMenuItem { Title = "Index", PageType = typeof(Pages.Home.IndexPage) },
                    new SubMenuItem { Title = "Home 2", PageType = typeof(Pages.Home.Home2Page) },
                    new SubMenuItem { Title = "Home 3" }
                },
                new List<SubMenuItem>()
                {
                    new SubMenuItem { Title = "Index", PageType = typeof(Pages.Settings.IndexPage) },
                    new SubMenuItem { Title = "Pressure" },
                    new SubMenuItem { Title = "Humidity" }
                },

        };

        private List<SubMenuItem> _subMenulist;
        public List<SubMenuItem> SubMenuList { get { return _subMenulist; } set { _subMenulist = value; Notify(); } }


        private List<MainMenuItem> mainMenu = new List<MainMenuItem>()
        {
            new MainMenuItem { Label = "Home", Icon = "Home", PageType=typeof(Pages.Home.IndexPage) },
            new MainMenuItem { Label = "Settings", Icon = "Setting", PageType=typeof(Pages.Settings.IndexPage) }
        };

        public List<MainMenuItem> MainMenuList { get { return mainMenu; } set { mainMenu = value; } }
    }
}
