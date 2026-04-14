using Firebase.Database;
using Firebase.Database.Query;
using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace CareReminderApp.Services
{
    public class FirebaseDataService : IDataService
    {
        private readonly FirebaseClient _firebase;
        private const string FirebaseUrl = "https://remaindsdb-default-rtdb.europe-west1.firebasedatabase.app";

        public FirebaseDataService()
        {
            _firebase = new FirebaseClient(FirebaseUrl);
        }

        // --- User Management ---

        public async Task<User> GetUserAsync(string userEmail, string password)
        {
            var users = await _firebase.Child("Users").OnceAsync<User>();

            return users
                .Select(u =>
                {
                    var user = u.Object;
                    user.Id = u.Key; // 🔥 חשוב
                    return user;
                })
                .FirstOrDefault(u =>
                    u.UserEmail?.Trim().ToLower() == userEmail.Trim().ToLower() &&
                    u.UserPassword == password);
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile, UserRole role)
        {
            try
            {
                var newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserEmail = userEmail.Trim().ToLower(),
                    UserPassword = password,
                    Mobile = mobile,
                    Role = role
                };

                var result = await _firebase.Child("Users").PostAsync(newUser);

                // 🔥 זה ה-Id האמיתי
                newUser.Id = result.Key;

                // עדכון המשתמש עם ה-Id
                await _firebase.Child("Users").Child(result.Key).PutAsync(newUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

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

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _firebase.Child("Users").Child(id).OnceSingleAsync<User>();
            if (user != null)
                user.Id = id;

            return user;
        }

        public async Task<User> FindSeniorByEmailAsync(string email)
        {
            var users = await _firebase.Child("Users").OnceAsync<User>();

            if (users == null || users.Count == 0)
                return null;

            string searchEmail = email.Trim().ToLower();

            foreach (var u in users)
            {
                var user = u.Object;
                user.Id = u.Key;

                if (user.UserEmail?.Trim().ToLower() == searchEmail &&
                    user.Role == UserRole.Senior)
                {
                    return user;
                }
            }

            return null;
        }
        // --- Reminder Management ---

        public async Task AddReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Id))
                reminder.Id = Guid.NewGuid().ToString();

            await _firebase.Child("Reminders").PostAsync(reminder);
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) =>
            (await GetRemindersAsync(userId)).ToList();

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId)
        {
            var reminders = await _firebase.Child("Reminders").OnceAsync<Reminder>();

            return reminders
                .Select(r => r.Object)
                .Where(r => r.UserId == userId);
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            var reminders = await _firebase.Child("Reminders").OnceAsync<Reminder>();

            var toUpdate = reminders.FirstOrDefault(r => r.Object.Id == reminder.Id);

            if (toUpdate != null)
            {
                await _firebase.Child("Reminders").Child(toUpdate.Key).PutAsync(reminder);
            }
        }

        // --- Connection Management ---

        public async Task AddUserConnectionAsync(string familyId, string seniorId)
        {
            var connection = new UserConnection
            {
                UserId = familyId,          // 🔥 חייב להיות Firebase Key
                ConnectedUserId = seniorId // 🔥 חייב להיות Firebase Key
            };

            await _firebase.Child("UserConnections").PostAsync(connection);
        }



        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId)
        {
            var connections = await _firebase.Child("UserConnections").OnceAsync<UserConnection>();

            return connections
                .Select(c => c.Object)
                .Where(c => c.UserId == userId || c.ConnectedUserId == userId)
                .ToList();
        }

        // --- Helpers ---

        public async Task<List<UserRole>> GetRolesAsync() =>
            await Task.FromResult(new List<UserRole>
            {
                UserRole.Senior,
                UserRole.FamilyMember
            });

        public async Task InviteElderAsync(string familyId, string elderId)
        {
            await _firebase.Child("PendingConnections")
                .PostAsync(new PendingConnection
                {
                    Id = Guid.NewGuid().ToString(),
                    FamilyId = familyId,
                    ElderId = elderId,
                    IsApproved = false
                });
        }

        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId)
        {
            var data = await _firebase.Child("PendingConnections")
                .OnceAsync<PendingConnection>();

            return data
                .Select(x => x.Object)
                .Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected);
        }

        public async Task ApproveConnectionAsync(PendingConnection request)
        {
            // 1. עדכון הסטטוס ב-PendingConnections ל-Approved
            var requests = await _firebase.Child("PendingConnections").OnceAsync<PendingConnection>();
            var toUpdate = requests.FirstOrDefault(x => x.Object.FamilyId == request.FamilyId && x.Object.ElderId == request.ElderId);

            if (toUpdate != null)
            {
                request.IsApproved = true;
                await _firebase.Child("PendingConnections").Child(toUpdate.Key).PutAsync(request);
            }

            // 2. יצירת הקשר הקבוע ב-UserConnections
            // 🔥 שינוי כאן: ודאי שהשמות תואמים למה ש-GetEldersForFamilyAsync מחפש
            var connection = new UserConnection
            {
                UserId = request.FamilyId,       // מזהה המשפחה
                ConnectedUserId = request.ElderId // מזהה המבוגר
            };

            await _firebase.Child("UserConnections").PostAsync(connection);
        }

        public async Task RejectConnectionAsync(PendingConnection request)
        {
            request.IsRejected = true;

            await _firebase.Child("PendingConnections")
                .Child(request.Id)
                .PutAsync(request);
        }

        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            try
            {
                // שליפת כל הקשרים
                var connections = await _firebase
                    .Child("UserConnections")
                    .OnceAsync<UserConnection>();

                var connectionsList = connections.Select(x => x.Object).ToList();

                // מציאת כל ה-IDs של המבוגרים (לא משנה כיוון הקשר)
                var elderIds = connectionsList
                    .Where(x => x.UserId == familyId || x.ConnectedUserId == familyId)
                    .Select(x => x.UserId == familyId ? x.ConnectedUserId : x.UserId)
                    .Distinct()
                    .ToList();

                if (!elderIds.Any())
                    return new List<User>();

                // שליפת כל המשתמשים
                var users = await _firebase
                    .Child("Users")
                    .OnceAsync<User>();

                // התאמה לפי KEY (הדבר הכי חשוב!)
                var result = users
                    .Select(u =>
                    {
                        var user = u.Object;
                        user.Id = u.Key; // 🔥 תמיד לקחת את ה-Key האמיתי
                        return user;
                    })
                    .Where(u => elderIds.Contains(u.Id))
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                return new List<User>();
            }
        }
    }
}