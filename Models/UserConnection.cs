using System;

namespace CareReminderApp.Models
{
    // מחלקה זו מייצגת חיבור בין שני משתמשים במערכת - בן משפחה וקשיש
    // ומאפשרת לשמור קשר קבוע בין משתמשים לאחר אישור החיבור
    public class UserConnection
    {
        // מזהה ייחודי של החיבור
        public string Id { get; set; }

        // מזהה המשתמש היוזם או בעל הקשר
        public string UserId { get; set; }

        // מזהה המשתמש המחובר (הצד השני בקשר)
        public string ConnectedUserId { get; set; }

        // תאריך ושעת יצירת החיבור בפורמט תקני של תאריך ושעה
        public string CreatedAt { get; set; } = DateTime.Now.ToString("o");
    }
}