using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public enum Operator
    {
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal
    }

    public class ThresholdConfig
    {
        public Operator Operator { get; set; }
        public double Value { get; set; }
        public ReportTemplate Template { get; set; }
    }
}
