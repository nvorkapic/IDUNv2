using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class FaultReportListingViewModel : NotifyBase
    {
        public List<FaultReport> Reports { get; set; }

        private FaultReport selectedReport;
        public FaultReport SelectedReport
        {
            get { return selectedReport; }
            set { selectedReport = value; Notify(); }
        }
    }
}
