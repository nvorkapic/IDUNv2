using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public class SelectedTrigger
    {
        public string Sensor { get; set; }
        public string Comparer { get; set; }
        public string Value { get; set; }
        public string TemplateName { get; set; }
        public string Symptom { get; set; }
        public string Priority { get; set; }
        public string Discovery { get; set; }
    }
}
