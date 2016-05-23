using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models;
using Addovation.Cloud.Apps.AddoResources.Client.Portable;

namespace IDUNv2.Services
{
    public interface IReportService
    {
        Task<List<ReportTemplate>> GetTemplates();
        Task<ReportTemplate> SetTemplate(ReportTemplate template);
        Task<List<FaultReport>> GetFaultReports();
        Task<FaultReport> SetFaultReport(FaultReport report);
    }
}
