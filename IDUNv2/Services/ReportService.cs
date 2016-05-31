using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models;
using System.IO;
using Windows.Storage;
using Newtonsoft.Json;
using SQLite.Net;

namespace IDUNv2.Services
{
    public class ReportService : IReportService
    {
        private static SQLiteConnection db = new SQLiteConnection(AppData.SqlitePlatform, AppData.DbPath);

        private CloudClient cloudClient;

        public ReportService()
        {
            this.cloudClient = AppData.CloudClient;
            db.CreateTable<ReportTemplate>();
        }

        public Task<List<ReportTemplate>> GetTemplates()
        {
            var templates = db.Table<ReportTemplate>().ToList();
            return Task.FromResult(templates);
        }

        public Task<ReportTemplate> FindTemplate(int Id)
        {
            var template= db.Find<ReportTemplate>(Id);
            return Task.FromResult(template);
        }

        public Task<ReportTemplate> SetTemplate(ReportTemplate template)
        {
            if (template.Id == 0)
                db.Insert(template);
            else
                db.Update(template);
            return Task.FromResult(template);
        }

        public async Task<List<FaultReport>> GetFaultReports()
        {
            return await cloudClient.GetFaultReports().ConfigureAwait(false);
        }

        public Task<FaultReport> SetFaultReport(FaultReport report)
        {
            throw new NotImplementedException();
        }
    }
}
