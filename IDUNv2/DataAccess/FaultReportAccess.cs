using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models;
using SQLite.Net;

namespace IDUNv2.DataAccess
{
    public class FaultReportAccess : IFaultReportAccess
    {
        private CachingCloudClient cloud;
        private SQLiteConnection db;

        public FaultReportAccess(CachingCloudClient cloud, SQLiteConnection db)
        {
            this.cloud = cloud;
            this.db = db;
        }

        public async Task FillCaches()
        {
            await cloud.FillCaches();
        }

        public Task<List<FaultReportTemplate>> GetFaultReportTemplates()
        {
            var templates = db.Table<FaultReportTemplate>().ToList();
            return Task.FromResult(templates);
        }

        public Task<FaultReportTemplate> FindFaultReportTemplate(int Id)
        {
            var template = db.Find<FaultReportTemplate>(Id);
            return Task.FromResult(template);
        }

        public Task<FaultReportTemplate> SetFaultReportTemplate(FaultReportTemplate template)
        {
            if (template.Id == 0)
                db.Insert(template);
            else
                db.Update(template);
            return Task.FromResult(template);
        }

        public Task<bool> DeleteFaultReportTemplate(FaultReportTemplate template)
        {
            var q = db.Table<SensorTrigger>()
                .Where(sr => sr.TemplateId == template.Id);
            if (q.Count() != 0)
            {
                return Task.FromResult(false);
            }
            db.Delete(template);
            return Task.FromResult(true);
        }

        public List<WorkOrderDiscCode> GetWorkOrderDiscCodes()
        {
            try
            {
                return cloud.GetCachedWorkOrderDiscCodes();
            }
            catch (Exception)
            {
                return new List<WorkOrderDiscCode>();
            }
        }

        public List<WorkOrderSymptCode> GetWorkOrderSymptCodes()
        {
            try
            {
                return cloud.GetCachedWorkOrderSymptCodes();
            }
            catch (Exception)
            {
                return new List<WorkOrderSymptCode>();
            }
        }

        public List<MaintenancePriority> GetWorkOrderPrioCodes()
        {
            try
            {
                return cloud.GetCachedMaintenancePriorities();
            }
            catch (Exception)
            {
                return new List<MaintenancePriority>();
            }
        }

        public WorkOrderDiscCode LookupWorkOrderDiscovery(string discCode)
        {
            return cloud.GetCachedWorkOrderDiscCode(discCode);
        }

        public WorkOrderSymptCode LookupWorkOrderSymptom(string symptCode)
        {
            return cloud.GetCachedWorkOrderSymptCode(symptCode);
        }

        public MaintenancePriority LookupMaintenancePriority(string prioCode)
        {
            return cloud.GetCachedMaintenancePriority(prioCode);
        }

        public async Task<List<FaultReport>> GetFaultReports(string mchCode = null)
        {
            var reports = await cloud.GetFaultReports();
            if (mchCode == null || mchCode == "")
                return reports;
            else
                return reports.Where(r => r.MchCode == mchCode).ToList();
        }

        public Task<FaultReport> SetFaultReport(FaultReport report)
        {
            return cloud.SetFaultReport(report);
        }
    }
}
