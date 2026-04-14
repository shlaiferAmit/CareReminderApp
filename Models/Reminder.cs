using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CareReminderApp.Models
{
    public class Reminder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;

        // שדה עזר לתצוגה ב-XAML כדי שלא יקרוס
        public string Time => DueDate.ToString("HH:mm");

        public bool IsCompleted { get; set; }
    }
}