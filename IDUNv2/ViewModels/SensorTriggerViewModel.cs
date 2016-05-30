using IDUNv2.Common;
using IDUNv2.Models;
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
        public SensorType[] Types { get; set; }
        public List<ReportTemplate> Templates { get; set; }

        ReportService RS = new ReportService();
        SensorService SS = new SensorService();

        public SensorTriggerViewModel()
        {
            Comparers = Enum.GetValues(typeof(SensorTriggerComparer)).Cast<SensorTriggerComparer>().ToArray();
            Types = Enum.GetValues(typeof(SensorType)).Cast<SensorType>().ToArray();
            Templates = RS.GetTemplates().Result;
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

