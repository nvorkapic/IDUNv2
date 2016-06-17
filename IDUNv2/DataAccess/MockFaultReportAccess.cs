using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using IDUNv2.Models;

namespace IDUNv2.DataAccess
{
    public class MockFaultReportAccess : IFaultReportAccess
    {
        private List<WorkOrderDiscCode> discCodes;
        private List<WorkOrderSymptCode> symptCodes;
        private List<MaintenancePriority> prioCodes;
        private List<FaultReportTemplate> templates;
        private List<FaultReport> faultReports;

        public Task FillCaches()
        {
            discCodes = new List<WorkOrderDiscCode>()
            {
                new WorkOrderDiscCode { ErrDiscoverCode = "1", Description = "Disc 1" },
                new WorkOrderDiscCode { ErrDiscoverCode = "2", Description = "Disc 2" },
                new WorkOrderDiscCode { ErrDiscoverCode = "3", Description = "Disc 3" },
            };
            symptCodes = new List<WorkOrderSymptCode>()
            {
                new WorkOrderSymptCode { ErrSymptom = "1", Description = "Sympt 1" },
                new WorkOrderSymptCode { ErrSymptom = "2", Description = "Sympt 2" },
                new WorkOrderSymptCode { ErrSymptom = "3", Description = "Sympt 3" },
            };
            prioCodes = new List<MaintenancePriority>()
            {
                new MaintenancePriority { PriorityId = "1", Description = "Prio 1" },
                new MaintenancePriority { PriorityId = "2", Description = "Prio 2" },
                new MaintenancePriority { PriorityId = "3", Description = "Prio 3" },
            };
            templates = new List<FaultReportTemplate>()
            {
                new FaultReportTemplate { Id = 1, Name = "Test 1", DiscCode = "1", SymptCode = "1", PrioCode = "1" },
                new FaultReportTemplate { Id = 2, Name = "Test 2", DiscCode = "2", SymptCode = "2", PrioCode = "2" },
                new FaultReportTemplate { Id = 3, Name = "Test 3", DiscCode = "3", SymptCode = "3", PrioCode = "3" },
            };
            faultReports = new List<FaultReport>()
            {
                new FaultReport
                {
                    Contract = "Contract 1",
                    ErrDescr = "ErrDescr 1",
                    ErrDescrLo = "ErrDescrLo 1",
                    ErrDiscoverCode = "1",
                    ErrSymptom = "1",
                    PriorityId = "1",
                    MchCode = "MchCode 1",
                    MchCodeContract = "MchCodeContract 1",
                    MchName = "MchName 1",
                    OrgCode = "OrgCode 1",
                    WoNo = 1,
                    RegDate = DateTime.Now
                },
                new FaultReport
                {
                    Contract = "Contract 2",
                    ErrDescr = "ErrDescr 2",
                    ErrDescrLo = "ErrDescrLo 2",
                    ErrDiscoverCode = "2",
                    ErrSymptom = "2",
                    PriorityId = "2",
                    MchCode = "MchCode 2",
                    MchCodeContract = "MchCodeContract 2",
                    MchName = "MchName 2",
                    OrgCode = "OrgCode 2",
                    WoNo = 2,
                    RegDate = DateTime.Now
                },
                new FaultReport
                {
                    Contract = "Contract 3",
                    ErrDescr = "ErrDescr 3",
                    ErrDescrLo = "ErrDescrLo 3",
                    ErrDiscoverCode = "3",
                    ErrSymptom = "3",
                    PriorityId = "3",
                    MchCode = "MchCode 3",
                    MchCodeContract = "MchCodeContract 3",
                    MchName = "MchName 3",
                    OrgCode = "OrgCode 3",
                    WoNo = 3,
                    RegDate = DateTime.Now
                },
            };

            return Task.Delay(1000);
        }

        public Task<bool> DeleteFaultReportTemplate(FaultReportTemplate template)
        {
            bool result = templates.Remove(template);
            return Task.FromResult(result);
        }

        public Task<FaultReportTemplate> FindFaultReportTemplate(int Id)
        {
            var template = templates.SingleOrDefault(t => t.Id == Id);
            return Task.FromResult(template);
        }

        public Task<List<FaultReport>> GetFaultReports()
        {
            return Task.FromResult(faultReports);
        }

        public Task<List<FaultReportTemplate>> GetFaultReportTemplates()
        {
            return Task.FromResult(templates);
        }

        public List<WorkOrderDiscCode> GetWorkOrderDiscCodes()
        {
            return discCodes;
        }

        public List<MaintenancePriority> GetWorkOrderPrioCodes()
        {
            return prioCodes;
        }

        public List<WorkOrderSymptCode> GetWorkOrderSymptCodes()
        {
            return symptCodes;
        }

        public MaintenancePriority LookupMaintenancePriority(string prioCode)
        {
            return prioCodes.SingleOrDefault(p => p.PriorityId == prioCode);
        }

        public WorkOrderDiscCode LookupWorkOrderDiscovery(string discCode)
        {
            return discCodes.SingleOrDefault(d => d.ErrDiscoverCode == discCode);
        }

        public WorkOrderSymptCode LookupWorkOrderSymptom(string symptCode)
        {
            return symptCodes.SingleOrDefault(s => s.ErrSymptom == symptCode);
        }

        public Task<FaultReport> SetFaultReport(FaultReport report)
        {
            throw new NotImplementedException();
        }

        public Task<FaultReportTemplate> SetFaultReportTemplate(FaultReportTemplate template)
        {
            template.Id = templates.Count + 1;
            templates.Add(template);
            return Task.FromResult(template);
        }
    }
}
