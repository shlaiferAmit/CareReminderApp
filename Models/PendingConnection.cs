using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    // מחלקה זו מייצגת בקשת חיבור בין בן משפחה לקשיש במערכת
    // הבקשה נשמרת במסד הנתונים עד לאישור או דחייה
    public class PendingConnection
    {
        // מזהה ייחודי של בקשת החיבור
        public string Id { get; set; } = string.Empty;

        // מזהה המשתמש של בן המשפחה ששלח את הבקשה
        public string FamilyId { get; set; } = string.Empty;

        // מזהה המשתמש של הקשיש שאליו נשלחה הבקשה
        public string ElderId { get; set; } = string.Empty;

        // מציין האם הבקשה אושרה
        public bool IsApproved { get; set; } = false;

        // מציין האם הבקשה נדחתה
        public bool IsRejected { get; set; } = false;

        // תאריך ושעת יצירת בקשת החיבור
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}