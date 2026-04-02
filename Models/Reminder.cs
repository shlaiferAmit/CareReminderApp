using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CareReminderApp.Models
{
    public partial class Reminder : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        // שימוש במאפיינים רגילים כדי למנוע שגיאות קומפילציה ב-Service
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}