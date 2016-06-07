using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public enum SensorTriggerComparer
    {
        Above,
        Below
    }

    public class SensorTrigger
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int TemplateId { get; set; }
        [NotNull]
        public SensorTriggerComparer Comparer { get; set; }
        [NotNull]
        public float Value { get; set; }
    }
}
