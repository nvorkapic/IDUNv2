using IDUNv2.Common;
using IDUNv2.Models;
using IDUNv2.Sensors;
using IDUNv2.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SensorTriggerViewModel : NotifyBase
    {
        public List<SensorTrigger> SensorTriggerList { get { return SS.GetTriggers().Result; } set { } }

        public SensorTriggerComparer[] Comparers { get; set; }
        public List<ReportTemplate> Templates { get; set; }

        SensorService SS = new SensorService();

        public SensorTriggerViewModel()
        {
            Comparers = Enum.GetValues(typeof(SensorTriggerComparer)).Cast<SensorTriggerComparer>().ToArray();
            Templates = AppData.Reports.GetTemplates().Result;
            CurrentTrigger = new SensorTrigger();
        }

        private SensorTrigger currentTrigger;
        public SensorTrigger CurrentTrigger
        {
            get { return currentTrigger; }
            set { currentTrigger = value; Notify(); }
        }

        internal void AddTrigger()
        {
            SS.InsertTrigger(CurrentTrigger);
        }
    }
}

