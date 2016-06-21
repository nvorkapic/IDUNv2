using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    class FaultReportDetailsViewModel : NotifyBase
    {
        #region Fields

        private IFaultReportAccess faultReportAccess;

        #endregion

        #region Notify Fields

        private FaultReport model;

        #endregion

        #region Notify Properties

        public FaultReport Model
        {
            get { return model; }
            set { model = value; Notify(); }
        }

        #endregion

        #region Properties

        public string Discovery
        {
            get { return faultReportAccess.LookupWorkOrderDiscovery(model.ErrDiscoverCode)?.Description; }
        }

        public string Symptom
        {
            get { return faultReportAccess.LookupWorkOrderSymptom(model.ErrSymptom)?.Description; }
        }

        public string Priority
        {
            get { return faultReportAccess.LookupMaintenancePriority(model.PriorityId)?.Description; }
        }

        #endregion

        public FaultReportDetailsViewModel(IFaultReportAccess faultReportAccess)
        {
            this.faultReportAccess = faultReportAccess;
        }
    }
}
