using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class ReportDetailViewModel : NotifyBase
    {
        private FaultReport details;
        public FaultReport Details
        {
            get { return details; }
            set { details = value; Notify(); }
        }

        public ReportDetailViewModel(FaultReport report)
        {
            Details = report;
        }
    }
}
