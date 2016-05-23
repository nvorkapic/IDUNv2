using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public enum NotificationType
    {
        Warning,
        Information,
        Error,
        Tooltip
    }
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Date { get; set; }
    }
}
