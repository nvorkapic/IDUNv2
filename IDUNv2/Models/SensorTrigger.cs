using IDUNv2.SensorLib;
using SQLite.Net.Attributes;

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
        [NotNull]
        public SensorId SensorId { get; set; }
        [Indexed]
        public int TemplateId { get; set; }
        [NotNull]
        public SensorTriggerComparer Comparer { get; set; }
        [NotNull]
        public float Value { get; set; }

        public override string ToString()
        {
            return $"[{Id}]: WHEN {Value} IS {Comparer.ToString().ToUpper()} USE {TemplateId}";
        }
    }
}
