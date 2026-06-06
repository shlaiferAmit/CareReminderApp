using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace CareReminderApp.Services
{
    // ממשק זה מגדיר את כל הפעולות האפשריות בשכבת הנתונים של האפליקציה
    // הוא משמש כשכבת ביניים בין שכבות הממשק לבין שירותי בסיס הנתונים כמו פיירבייס
    // ומאפשר להחליף מימושים מבלי לשנות את שאר חלקי האפליקציה
    public interface IDataService
    {
        // ניהול משתמשים

        // קבלת משתמש לפי אימייל וסיסמה
        Task<User> GetUserAsync(string email, string password);

        // רישום משתמש חדש במערכת
        Task<bool> RegisterUserAsync(string id, string firstName, string lastName, string email, string password, string mobile, UserRole role);

        // קבלת כל המשתמשים במערכת
        Task<List<User>> GetUsersAsync();

        // קבלת משתמש לפי מזהה ייחודי
        Task<User?> GetUserByIdAsync(string id);


        // תזכורות

        // קבלת כל התזכורות של משתמש לפי מזהה
        Task<List<Reminder>> GetRemindersByUserIdAsync(string userId);

        // קבלת תזכורות של משתמש (גרסה כללית)
        Task<IEnumerable<Reminder>> GetRemindersAsync(string userId);

        // שמירת תזכורת חדשה
        Task SaveReminderAsync(Reminder reminder);

        // עדכון תזכורת קיימת
        Task UpdateReminderAsync(Reminder reminder);

        // מחיקת תזכורת לפי מזהה
        Task<bool> DeleteReminderAsync(string id);


        // קשרים

        // קבלת כל הקשרים של משתמש
        Task<List<UserConnection>> GetUserConnectionsAsync(string userId);

        // קבלת כל הקשישים המשויכים לבן משפחה
        Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId);

        // יצירת קשר בין בן משפחה לקשיש
        Task AddUserConnectionAsync(string familyId, string seniorId);

        // הסרת קשר בין בן משפחה לקשיש
        Task RemoveUserConnectionAsync(string familyId, string seniorId);


        // בקשות אישור

        // שליחת הזמנה לחיבור בין משתמשים
        Task InviteElderAsync(string familyId, string elderId);

        // קבלת בקשות חיבור ממתינות לקשיש
        Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId);

        // אישור בקשת חיבור
        Task ApproveConnectionAsync(PendingConnection request);

        // דחיית בקשת חיבור
        Task RejectConnectionAsync(PendingConnection request);

        // קבלת רשימת סוגי המשתמשים במערכת
        Task<List<UserRole>> GetRolesAsync();

        // עדכון פרטי משתמש קיים
        Task<bool> UpdateUserAsync(User user);

        // העלאת תמונת פרופיל של משתמש
        Task<string> UploadUserImageAsync(Stream imageStream, string userId);


        // 🌟 האזנות בזמן אמת (Realtime Listeners) 🌟

        // האזנה לקשישים המשויכים לבן משפחה
        IObservable<List<User>> ListenEldersForFamily(string familyId);

        // האזנה לתזכורות של קשיש (שונה ל-List<Reminder> כדי להתאים בדיוק ל-ViewModel)
        IObservable<List<Reminder>> ListenRemindersForElder(string elderId);

        // האזנה לבקשות חיבור ממתינות עבור קשיש
        IObservable<IEnumerable<PendingConnection>> ListenPendingConnectionsForElder(string elderId);
    }
}