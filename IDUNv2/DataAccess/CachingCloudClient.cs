using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    /// <summary>
    /// CloudClient augmented with some simple caching of rarely changing list data
    /// </summary>
    public class CachingCloudClient : CloudClient
    {
        #region Fields

        private List<WorkOrderDiscCode> woDiscCodes;
        private List<WorkOrderSymptCode> woSymptCodes;
        private List<MaintenancePriority> woPrioCodes;
        private Dictionary<string, WorkOrderDiscCode> woDiscDict;
        private Dictionary<string, WorkOrderSymptCode> woSymptDict;
        private Dictionary<string, MaintenancePriority> woPrioDict;

        #endregion

        #region Properties

        public override string PlatformName => "iOS";
        public override string PlatformVersion => "9.1";
        public override string AppVersion => "2.0.1";
        public override string DeviceId { get; } = Guid.NewGuid().ToString();
        public override string AppId => "TestClient";

        #endregion

        protected override void OnError(string error, Exception ex)
        {
            Debug.WriteLine(error);
            //throw ex;
        }

        #region Caching augmentation

        public WorkOrderDiscCode GetCachedWorkOrderDiscCode(string discCode)
        {
            if (discCode == null || !woDiscDict.ContainsKey(discCode))
                return null;
             return woDiscDict[discCode];
        }

        public WorkOrderSymptCode GetCachedWorkOrderSymptCode(string symptCode)
        {
            if (symptCode == null || !woSymptDict.ContainsKey(symptCode))
                return null;
            return woSymptDict[symptCode];
        }

        public MaintenancePriority GetCachedMaintenancePriority(string prioCode)
        {
            if (prioCode == null || !woPrioDict.ContainsKey(prioCode))
                return null;
            return woPrioDict[prioCode];
        }

        public List<WorkOrderDiscCode> GetCachedWorkOrderDiscCodes()
        {
            return woDiscCodes;
        }

        public List<WorkOrderSymptCode> GetCachedWorkOrderSymptCodes()
        {
            return woSymptCodes;
        }

        public List<MaintenancePriority> GetCachedMaintenancePriorities()
        {
            return woPrioCodes;
        }

        /// <summary>
        /// If caches are empty fill them up with data from database
        /// </summary>
        /// <returns></returns>
        public async Task FillCaches()
        {
            woDiscCodes = woDiscCodes ?? await GetWorkOrderDiscCodes().ConfigureAwait(false);
            woSymptCodes = woSymptCodes ?? await GetWorkOrderSymptCodes().ConfigureAwait(false);
            woPrioCodes = woPrioCodes ?? await GetMaintenancePriorities().ConfigureAwait(false);

            woDiscDict = woDiscDict ?? woDiscCodes.ToDictionary(k => k.ErrDiscoverCode, e => e);
            woSymptDict = woSymptDict ?? woSymptCodes.ToDictionary(k => k.ErrSymptom, e => e);
            woPrioDict = woPrioDict ?? woPrioCodes.ToDictionary(k => k.PriorityId, e => e);
        }

        #endregion
    }
}
