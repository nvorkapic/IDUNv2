using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models.Reports;
using System.IO;
using Windows.Storage;

namespace IDUNv2.Services
{
    public class FaultReportService
    {
        private CloudClient cloudClient;

        private List<Models.Reports.TemplateModel> _reportTemplates = new List<Models.Reports.TemplateModel>
        {
            new Models.Reports.TemplateModel { Name = "Template 1" },
            new Models.Reports.TemplateModel { Name = "Template 2" },
            new Models.Reports.TemplateModel { Name = "Template 3" },
        };

        public List<WorkOrderDiscCode> DiscCodes { get; private set; }
        public List<WorkOrderSymptCode> SymptCodes { get; private set; }
        public List<MaintenancePriority> PrioCodes { get; private set; }

        public FaultReportService(CloudClient cloudClient)
        {
            this.cloudClient = cloudClient;

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            using (var db = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<TemplateModel>();
                var tm = db.GetMapping<TemplateModel>();

                var i = db.Insert(new TemplateModel { Name = "Template 1" });
                var ts = (from t in db.Table<TemplateModel>()
                          select t).ToList();
                var name = ts.FirstOrDefault()?.Name;
            }
        }

        public List<Models.Reports.TemplateModel> GetFaultReportTemplates()
        {
            return _reportTemplates;
        }

        public TemplateModel AddTemplate(TemplateModel template)
        {
            _reportTemplates.Add(template);
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
