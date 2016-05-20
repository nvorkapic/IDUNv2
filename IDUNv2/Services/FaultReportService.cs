using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models;
using System.IO;
using Windows.Storage;

namespace IDUNv2.Services
{
    public class FaultReportService
    {
        private CloudClient cloudClient;

        private List<ReportTemplate> _templates = new List<ReportTemplate>
        {
            new ReportTemplate { Name = "Template 1" },
            new ReportTemplate { Name = "Template 2" },
            new ReportTemplate { Name = "Template 3" },
        };

        public List<WorkOrderDiscCode> DiscCodes { get; private set; }
        public List<WorkOrderSymptCode> SymptCodes { get; private set; }
        public List<MaintenancePriority> PrioCodes { get; private set; }

        public FaultReportService(CloudClient cloudClient)
        {
            this.cloudClient = cloudClient;
        }

        public List<ReportTemplate> GetFaultReportTemplates()
        {
            return _templates;
        }

        public ReportTemplate AddTemplate(ReportTemplate template)
        {
            _templates.Add(template);
            return template;
        }

        public async Task<List<FaultReport>> GetFaultReports()
        {
            return await cloudClient.GetFaultReports().ConfigureAwait(false);
        }

        private static async Task<List<T>> GetCachedList<T>(Func<Task<List<T>>> getFunc, List<T> cachedList, bool useCached)
        {
            List<T> results = cachedList;
            if (results == null || !useCached)
            {
                results = await getFunc().ConfigureAwait(false);
            }
            return results;
        }

        public async Task<List<WorkOrderDiscCode>> GetDiscCodes(bool useCached = true)
        {
            DiscCodes = await GetCachedList(cloudClient.GetWorkOrderDiscCodes, DiscCodes, useCached).ConfigureAwait(false);
            return DiscCodes;
        }

        public async Task<List<WorkOrderSymptCode>> GetSymptCodes(bool useCached = true)
        {
            SymptCodes = await GetCachedList(cloudClient.GetWorkOrderSymptCodes, SymptCodes, useCached).ConfigureAwait(false);
            return SymptCodes;
        }

        public async Task<List<MaintenancePriority>> GetPrioCodes(bool useCached = true)
        {
            PrioCodes = await GetCachedList(cloudClient.GetMaintenancePriorities, PrioCodes, useCached).ConfigureAwait(false);
            return PrioCodes;
        }

        public async Task InitCaches()
        {
            DiscCodes = await GetDiscCodes(false).ConfigureAwait(false);
            SymptCodes = await GetSymptCodes(false).ConfigureAwait(false);
            PrioCodes = await GetPrioCodes(false).ConfigureAwait(false);
        }
    }
}
