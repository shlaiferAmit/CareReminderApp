using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CareReminderApp.Models
{
    // הוספת partial ו-ObservableObject כדי שה-Toolkit יעבוד
    public partial class Reminder : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // פותר את השגיאה ב-AddReminderViewModel
        public DateTime DueDate { get; set; } = DateTime.Now;

        // פותר את השגיאה ב-MockDataService (שורה 26)
        public DateTime Time { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; }
    }
}