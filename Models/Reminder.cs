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
        // מזהה ייחודי של התזכורת
        public string Id { get; set; } = string.Empty;

        // כותרת התזכורת
        public string Title { get; set; } = string.Empty;

        // תיאור מפורט של התזכורת
        public string Description { get; set; } = string.Empty;

        // תאריך ושעת היעד לביצוע התזכורת
        public DateTime DueDate { get; set; }

        // מזהה המשתמש שאליו שייכת התזכורת
        public string UserId { get; set; } = string.Empty;

        // מציין האם התזכורת הושלמה
        public bool IsCompleted { get; set; }
    }
}