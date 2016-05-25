using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public class MeasurementModel
    {
        public string MeasurementName { get; set; }

        public bool Enabled { get; set; }

        public double CurrentValue { get; set; }

        public double? CurrentVectorValueX { get; set; }
        public double? CurrentVectorValueY { get; set; }
        public double? CurrentVectorValueZ { get; set; }

        public ObservableCollection<ThresholdConfig> ThresholdList { get; set; }

    }
}
