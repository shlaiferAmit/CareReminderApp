using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

using Newtonsoft.Json;

namespace CareReminderApp.Models
{
    // מחלקה זו מייצגת תזכורת במערכת
    // כל תזכורת משויכת למשתמש מסוים וכוללת כותרת, תיאור ותאריך יעד
    public class Reminder
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}