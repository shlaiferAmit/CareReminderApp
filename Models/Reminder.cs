using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    public class Reminder
    {
        public string Id { get; set; }
        public string UserId { get; set; } // למי התזכורת שייכת
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
