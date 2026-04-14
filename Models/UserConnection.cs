using System;

namespace CareReminderApp.Models
{
    public class UserConnection
    {
        // ב-Firebase אין שדה Id בתוך האובייקט (זה ה-Key של הרשומה), 
        // אבל נשאיר אותו כאן למקרה שתצטרכי.
        public string Id { get; set; }

        // השדות האלו חייבים להתאים בדיוק לשמות בתמונה (UserId ו-ConnectedUserId)
        public string UserId { get; set; }

        public string ConnectedUserId { get; set; }

        // שינוי קריטי: הגדרה כ-string כדי למנוע שגיאות המרה מה-Firebase
        public string CreatedAt { get; set; } = DateTime.Now.ToString("o");
    }
}