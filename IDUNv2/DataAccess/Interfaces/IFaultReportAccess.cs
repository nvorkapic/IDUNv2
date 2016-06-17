using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDUNv2.DataAccess
{
    /// <summary>
    /// Retreiving and setting fault reports and fault report templates.
    /// </summary>
    public interface IFaultReportAccess
    {
        Task FillCaches();

        Task<List<FaultReportTemplate>> GetFaultReportTemplates();
        Task<FaultReportTemplate> FindFaultReportTemplate(int Id);
        Task<FaultReportTemplate> SetFaultReportTemplate(FaultReportTemplate template);
        Task<bool> DeleteFaultReportTemplate(FaultReportTemplate template);

        List<WorkOrderDiscCode> GetWorkOrderDiscCodes();
        List<WorkOrderSymptCode> GetWorkOrderSymptCodes();
        List<MaintenancePriority> GetWorkOrderPrioCodes();
        WorkOrderDiscCode LookupWorkOrderDiscovery(string discCode);
        WorkOrderSymptCode LookupWorkOrderSymptom(string symptCode);
        MaintenancePriority LookupMaintenancePriority(string prioCode);

        Task<List<FaultReport>> GetFaultReports();
        Task<FaultReport> SetFaultReport(FaultReport report);
    }
}
