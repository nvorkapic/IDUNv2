using IDUNv2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class AppBrushViewModel : NotifyBase
    {
        private string _brushesResources;
        public string BrushesResources
        {
            get { return _brushesResources; }
            set { _brushesResources = value; Notify(); }
        }
    }
}
