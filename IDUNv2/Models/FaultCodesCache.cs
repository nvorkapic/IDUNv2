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
            if (discCode == null) return null;
            try
            {
                return discDict[discCode];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public WorkOrderSymptCode GetSymptom(string symptCode)
        {
            if (symptCode == null) return null;
            try
            {
                return symptDict[symptCode];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public MaintenancePriority GetPriority(string prioCode)
        {
            if (prioCode == null) return null;
            try
            {
                return prioDict[prioCode];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
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

        public async Task InitAsync()
        {
            if (cloudClient == null)
            {
                DiscCodes = new List<WorkOrderDiscCode>();
                SymptCodes = new List<WorkOrderSymptCode>();
                PrioCodes = new List<MaintenancePriority>();
            }
            else
            {
                DiscCodes = await GetDiscCodes(false).ConfigureAwait(false);
                SymptCodes = await GetSymptCodes(false).ConfigureAwait(false);
                PrioCodes = await GetPrioCodes(false).ConfigureAwait(false);
            }

            discDict = DiscCodes.ToDictionary(ks => ks.ErrDiscoverCode, es => es);
            symptDict = SymptCodes.ToDictionary(ks => ks.ErrSymptom, es => es);
            prioDict = PrioCodes.ToDictionary(ks => ks.PriorityId, es => es);
        }

        public static async Task<FaultCodesCache> CreateAsync(CloudClient cloudClient)
        {
            var cache = new FaultCodesCache(cloudClient);
            await cache.InitAsync();
            return cache;
        }
    }
}
