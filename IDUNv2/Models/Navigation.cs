using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public class MainMenuItem 
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public List<SubMenuItem> SubMenu { get; set; }
    }

    public class SubMenuItem
    {
        public string Label { get; set; }
        public Type PageType { get; set; }
        public string Icon { get; set; }
    }
}
