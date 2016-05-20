using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models.Reports
{
    public class TemplateModel : ModelBase
    {
        #region PropertyFields
        private string _name;
        private string _directive;
        private string _faultDescr;
        private WorkOrderDiscCode _discovery;
        private WorkOrderSymptCode _symptom;
        private MaintenancePriority _priority;
        #endregion

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(64)]
        public string Name { get { return _name; } set { _name = value; Notify(); } }

        public string Directive { get { return _directive; } set { _directive = value; Notify(); } }
        public string FaultDescr { get { return _faultDescr; } set { _faultDescr = value; Notify(); } }

        [Ignore]
        public WorkOrderDiscCode Discovery { get { return _discovery; } set { _discovery = value; Notify(); } }

        [Ignore]
        public WorkOrderSymptCode Symptom { get { return _symptom; } set { _symptom = value; Notify(); } }

        [Ignore]
        public MaintenancePriority Priority { get { return _priority; } set { _priority = value; Notify(); } }
    }
}
