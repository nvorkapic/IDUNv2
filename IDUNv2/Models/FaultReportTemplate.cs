using SQLite.Net.Attributes;

namespace IDUNv2.Models
{
    public class FaultReportTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(64), NotNull]
        public string Name { get; set; }

        public string Directive { get; set; }
        public string FaultDescr { get; set; }
        public string DiscCode { get; set; }
        public string SymptCode { get; set; }
        public string PrioCode { get; set; }
    }
}
