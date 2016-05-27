using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public class FaultCodesCache
    {
        private CloudClient cloudClient;

        private static WorkOrderDiscCode EmptyDiscCode = new WorkOrderDiscCode { Description = "", ErrDiscoverCode = "" };
        private static WorkOrderSymptCode EmptySymptCode = new WorkOrderSymptCode { Description = "", ErrSymptom = "" };
        private static MaintenancePriority EmptyPrioCode = new MaintenancePriority { Description = "", PriorityId = "" };

        private Dictionary<string, WorkOrderDiscCode> discDict;
        private Dictionary<string, WorkOrderSymptCode> symptDict;
        private Dictionary<string, MaintenancePriority> prioDict;

        public List<WorkOrderDiscCode> DiscCodes { get; private set; }
        public List<WorkOrderSymptCode> SymptCodes { get; private set; }
        public List<MaintenancePriority> PrioCodes { get; private set; }

        private static async Task<List<T>> GetCachedList<T>(Func<Task<List<T>>> getFunc, List<T> cachedList, bool useCached)
        {
            List<T> results = cachedList;
            if (results == null || !useCached)
            {
                results = await getFunc().ConfigureAwait(false);
            }
            return results;
        }

        public WorkOrderDiscCode GetDiscovery(string discCode)
        {
            if (discCode == null) return EmptyDiscCode;
            return discDict[discCode];
        }

        public WorkOrderSymptCode GetSymptom(string symptCode)
        {
            if (symptCode == null) return EmptySymptCode;
            return symptDict[symptCode];
        }

        public MaintenancePriority GetPriority(string prioCode)
        {
            if (prioCode == null) return EmptyPrioCode;
            return prioDict[prioCode];
        }

        private async Task<List<WorkOrderDiscCode>> GetDiscCodes(bool useCached = true)
        {
            DiscCodes = await GetCachedList(cloudClient.GetWorkOrderDiscCodes, DiscCodes, useCached).ConfigureAwait(false);
            return DiscCodes;
        }

        private async Task<List<WorkOrderSymptCode>> GetSymptCodes(bool useCached = true)
        {
            SymptCodes = await GetCachedList(cloudClient.GetWorkOrderSymptCodes, SymptCodes, useCached).ConfigureAwait(false);
            return SymptCodes;
        }

        private async Task<List<MaintenancePriority>> GetPrioCodes(bool useCached = true)
        {
            PrioCodes = await GetCachedList(cloudClient.GetMaintenancePriorities, PrioCodes, useCached).ConfigureAwait(false);
            return PrioCodes;
        }

        private FaultCodesCache(CloudClient cloudClient)
        {
            this.cloudClient = cloudClient;
        }

        public static async Task<FaultCodesCache> CreateAsync(CloudClient cloudClient)
        {
            var cache = new FaultCodesCache(cloudClient);

            if (cloudClient == null)
            {
                cache.DiscCodes = new List<WorkOrderDiscCode>();
                cache.SymptCodes = new List<WorkOrderSymptCode>();
                cache.PrioCodes = new List<MaintenancePriority>();
            }
            else
            {
                cache.DiscCodes = await cache.GetDiscCodes(false).ConfigureAwait(false);
                cache.SymptCodes = await cache.GetSymptCodes(false).ConfigureAwait(false);
                cache.PrioCodes = await cache.GetPrioCodes(false).ConfigureAwait(false);
            }

            cache.discDict = cache.DiscCodes.ToDictionary(ks => ks.ErrDiscoverCode, es => es);
            cache.symptDict = cache.SymptCodes.ToDictionary(ks => ks.ErrSymptom, es => es);
            cache.prioDict = cache.PrioCodes.ToDictionary(ks => ks.PriorityId, es => es);

            return cache;
        }
    }
}
