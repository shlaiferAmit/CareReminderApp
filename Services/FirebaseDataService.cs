using CareReminderApp.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace CareReminderApp.Services
{
    // מחלקה זו אחראית על כל הפעולות מול בסיס הנתונים פיירבייס ריל טיים דטאבייס
    // כולל משתמשים, תזכורות, חיבורים בין משתמשים, ובקשות חיבור
    public class FirebaseDataService : IDataService
    {
        private readonly FirebaseClient _firebase;
        private readonly AuthService _authService;

        // כתובת בסיס של מסד הנתונים בפיירבייס
        private const string FirebaseUrl = "https://remaindsdb-default-rtdb.europe-west1.firebasedatabase.app";

        public FirebaseDataService(AuthService authService)
        {
            // יצירת חיבור למסד הנתונים של פיירבייס
            _firebase = new FirebaseClient(FirebaseUrl);
            _authService = authService;
        }

        // רישום משתמש חדש ושמירתו במסד הנתונים
        public async Task<bool> RegisterUserAsync(string id, string firstName, string lastName, string email, string password, string mobile, UserRole role)
        {
            try
            {
                // יצירת אובייקט משתמש חדש
                var newUser = new User
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    UserEmail = email.ToLower().Trim(),
                    UserPassword = password,
                    Mobile = mobile,
                    Role = role
                };

                // שמירת המשתמש תחת צומת משתמשים לפי מזהה ייחודי
                await _firebase
                    .Child("Users")
                    .Child(id)
                    .PutAsync(newUser);

                return true;
            }
            catch (Exception ex)
            {
                // הדפסת שגיאה לצורך בדיקה
                System.Diagnostics.Debug.WriteLine($"DB Error: {ex.Message}");
                return false;
            }
        }

        // קבלת משתמש לפי אימייל וסיסמה - חיפוש במסד הנתונים
        public async Task<User?> GetUserAsync(string email, string password)
        {
            try
            {
                var users = await _firebase
                    .Child("Users")
                    .OnceAsync<User>();

                var user = users
                    .Select(u =>
                    {
                        var currentUser = u.Object;
                        currentUser.Id = u.Key;
                        return currentUser;
                    })
                    .FirstOrDefault(u =>
                        u.UserEmail.ToLower().Trim() == email.ToLower().Trim());

                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUserAsync ERROR: {ex}");
                return null;
            }
        }

        // קבלת משתמש לפי מזהה ייחודי
        public async Task<User?> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.WriteLine("GetUserByIdAsync: ID is null or empty!");
                return null;
            }

            try
            {
                var user = await _firebase
                    .Child("Users")
                    .Child(id)
                    .OnceSingleAsync<User>();

                if (user != null)
                    user.Id = id;

                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUserByIdAsync DATABASE ERROR: {ex.Message}");
                return null;
            }
        }

        // קבלת כל המשתמשים במערכת
        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _firebase.Child("Users").OnceAsync<User>();
            return users.Select(u =>
            {
                var user = u.Object;
                user.Id = u.Key;
                return user;
            }).ToList();
        }

        // עדכון פרטי משתמש קים במסד הנתונים
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                await _firebase
                    .Child("Users")
                    .Child(user.Id)
                    .PutAsync(user);
                return true;
            }
            catch { return false; }
        }

        // העלאת תמונת פרופיל לפיירבייס סטורג'
        public async Task<string> UploadUserImageAsync(Stream imageStream, string userId)
        {
            var storage = new FirebaseStorage("remaindsdb.firebasestorage.app");
            var uploadTask = await storage
                .Child("ProfileImages")
                .Child($"{userId}.jpg")
                .PutAsync(imageStream);

            return uploadTask;
        }

        // קבלת כל התזכורות של משתמש מסוים
        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId)
        {
            var data = await _firebase.Child("Reminders").OnceAsync<Reminder>();
            return data.Select(x => { var r = x.Object; r.Id = x.Key; return r; })
                       .Where(r => r.UserId == userId).ToList();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) => await GetRemindersByUserIdAsync(userId);

        // שמירת תזכורת חדשה
        public async Task SaveReminderAsync(Reminder reminder) => await _firebase.Child("Reminders").PostAsync(reminder);

        // עדכון תזכורת קיימת
        public async Task UpdateReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Id)) return;
            await _firebase.Child("Reminders").Child(reminder.Id).PutAsync(reminder);
        }

        // מחיקת תזכורת מהמערכת
        public async Task<bool> DeleteReminderAsync(string reminderId)
        {
            try
            {
                await _firebase
                    .Child("Reminders")
                    .Child(reminderId)
                    .DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting reminder: {ex.Message}");
                return false;
            }
        }

        // יצירת קשר בין בן משפחה לקשיש
        public async Task AddUserConnectionAsync(string familyId, string seniorId)
        {
            await _firebase.Child("UserConnections").PostAsync(new UserConnection { UserId = familyId, ConnectedUserId = seniorId });
        }

        // קבלת כל הקשרים של משתמש
        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId)
        {
            var connections = await _firebase.Child("UserConnections").OnceAsync<UserConnection>();
            return connections.Select(c => c.Object).Where(c => c.UserId == userId || c.ConnectedUserId == userId).ToList();
        }

        // קבלת כל הקשישים של בן משפחה
        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            var connections = await GetUserConnectionsAsync(familyId);
            var ids = connections.Select(c => c.UserId == familyId ? c.ConnectedUserId : c.UserId);
            var allUsers = await GetUsersAsync();
            return allUsers.Where(u => ids.Contains(u.Id));
        }

        // שליחת הזמנה לקשר בין משתמשים
        public async Task InviteElderAsync(string familyId, string elderId)
        {
            var familyUser = await GetUserByIdAsync(familyId);

            await _firebase.Child("PendingConnections")
                .PostAsync(new PendingConnection
                {
                    FamilyId = familyId,
                    ElderId = elderId,
                    FamilyName = $"{familyUser.FirstName} {familyUser.LastName}"
                });
        }

        // קבלת בקשות חיבור ממתינות לקשיש
        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId)
        {
            var data = await _firebase.Child("PendingConnections").OnceAsync<PendingConnection>();
            return data.Select(x => { var p = x.Object; p.Id = x.Key; return p; })
                       .Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected);
        }

        // אישור בקשת חיבור
        public async Task ApproveConnectionAsync(PendingConnection request)
        {
            request.IsApproved = true;
            await _firebase.Child("PendingConnections").Child(request.Id).PutAsync(request);
            await AddUserConnectionAsync(request.FamilyId, request.ElderId);
        }

        // דחיית בקשת חיבור
        public async Task RejectConnectionAsync(PendingConnection request)
        {
            request.IsRejected = true;
            await _firebase.Child("PendingConnections").Child(request.Id).PutAsync(request);
        }

        // החזרת רשימת סוגי המשתמשים במערכת
        public async Task<List<UserRole>> GetRolesAsync() => new List<UserRole> { UserRole.Senior, UserRole.FamilyMember };

        // אופציה לניתוק קשר
        public async Task RemoveUserConnectionAsync(string familyId, string seniorId)
        {
            var connections = await _firebase
                .Child("UserConnections")
                .OnceAsync<UserConnection>();

            var item = connections.FirstOrDefault(c =>
                c.Object.UserId == familyId &&
                c.Object.ConnectedUserId == seniorId);

            if (item != null)
            {
                await _firebase
                    .Child("UserConnections")
                    .Child(item.Key)
                    .DeleteAsync();
            }
        }

        public IObservable<List<User>> ListenEldersForFamily(string familyId)
        {
            return Observable.Create<List<User>>(observer =>
            {
                var cancellation = new System.Threading.CancellationTokenSource();

                Task.Run(async () =>
                {
                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        try
                        {
                            var elders = (await GetEldersForFamilyAsync(familyId)).ToList();
                            observer.OnNext(elders);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }

                        try
                        {
                            await Task.Delay(3000, cancellation.Token);
                        }
                        catch (TaskCanceledException) { break; }
                    }
                }, cancellation.Token);

                return () => cancellation.Cancel();
            });
        }

        /// האזנה רציפה בזמן אמת לכל התזכורות של המבוגר - שונה ל-List למניעת Ambiguity ושגיאות קומפילציה
        public IObservable<List<Reminder>> ListenRemindersForElder(string elderId)
        {
            return Observable.Create<List<Reminder>>(observer =>
            {
                var cancellation = new System.Threading.CancellationTokenSource();

                Task.Run(async () =>
                {
                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // שליפת התזכורות העדכניות מהמסד כרשימה (List)
                            var reminders = await GetRemindersByUserIdAsync(elderId);
                            observer.OnNext(reminders);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Stream Error (Reminders): {ex.Message}");
                        }

                        try
                        {
                            // בדיקה כל 3 שניות בדומה למסך המשפחה
                            await Task.Delay(3000, cancellation.Token);
                        }
                        catch (TaskCanceledException) { break; }
                    }
                }, cancellation.Token);

                return () => cancellation.Cancel();
            });
        }

        /// האזנה רציפה בזמן אמת לבקשות חיבור ממתינות עבור המבוגר
        public IObservable<IEnumerable<PendingConnection>> ListenPendingConnectionsForElder(string elderId)
        {
            return Observable.Create<IEnumerable<PendingConnection>>(observer =>
            {
                var cancellation = new System.Threading.CancellationTokenSource();

                Task.Run(async () =>
                {
                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // שליפת בקשות החיבור הממתינות והלא מאושרות/דחויות
                            var requests = await GetPendingForElderAsync(elderId);
                            observer.OnNext(requests);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Stream Error (Pending Connections): {ex.Message}");
                        }

                        try
                        {
                            await Task.Delay(3000, cancellation.Token);
                        }
                        catch (TaskCanceledException) { break; }
                    }
                }, cancellation.Token);

                return () => cancellation.Cancel();
            });
        }
    }
}