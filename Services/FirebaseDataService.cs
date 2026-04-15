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

        public async Task<User?> GetUserAsync(string email, string password)
        {
            try
            {
                // שליפת כל המשתמשים כ-JObject כדי שנוכל לטפל בשמות שדות שונים (אותיות גדולות/קטנות)
                var allUsersData = await _firebase.Child("Users").OnceAsync<JObject>();
                if (allUsersData == null) return null;

                string searchEmail = email.Trim().ToLower();

                foreach (var item in allUsersData)
                {
                    var userJson = item.Object;

                    // בדיקה גמישה של שם השדה (UserEmail או userEmail)
                    string firebaseEmail = (userJson["UserEmail"] ?? userJson["userEmail"])?.ToString().Trim().ToLower() ?? "";
                    string firebasePass = (userJson["UserPassword"] ?? userJson["userPassword"])?.ToString().Trim() ?? "";

                    if (firebaseEmail == searchEmail)
                    {
                        return new User
                        {
                            Id = item.Key,
                            FirstName = (userJson["FirstName"] ?? userJson["firstName"])?.ToString() ?? "",
                            LastName = (userJson["LastName"] ?? userJson["lastName"])?.ToString() ?? "",
                            UserEmail = firebaseEmail,
                            Mobile = (userJson["Mobile"] ?? userJson["mobile"])?.ToString() ?? "",
                            Role = (UserRole)(userJson["Role"] ?? userJson["role"]).ToObject<int>()
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string email, string password, string mobile, UserRole role)
        {
            try
            {
                var newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserEmail = email.Trim().ToLower(),
                    UserPassword = password,
                    Mobile = mobile,
                    Role = role
                };

                // יצירת מזהה חדש ושמירה
                var result = await _firebase.Child("Users").PostAsync(newUser);
                newUser.Id = result.Key;

                // עדכון האובייקט עם ה-Id שנוצר
                await _firebase.Child("Users").Child(result.Key).PutAsync(newUser);
                return true;
            }
            catch { return false; }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _firebase.Child("Users").OnceAsync<User>();
            return users.Select(u => {
                var user = u.Object;
                user.Id = u.Key;
                return user;
            }).ToList();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var user = await _firebase.Child("Users").Child(id).OnceSingleAsync<User>();
            if (user != null) user.Id = id;
            return user;
        }

        public async Task<User?> FindSeniorByEmailAsync(string email)
        {
            var users = await _firebase.Child("Users").OnceAsync<JObject>();
            string searchEmail = email.Trim().ToLower();

            foreach (var u in users)
            {
                string uEmail = (u.Object["UserEmail"] ?? u.Object["userEmail"])?.ToString().Trim().ToLower() ?? "";
                if (uEmail == searchEmail)
                {
                    return new User { Id = u.Key, UserEmail = uEmail, Role = UserRole.Senior };
                }
            }
            return null;
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId)
        {
            var data = await _firebase.Child("Reminders").OnceAsync<Reminder>();
            return data.Select(x => {
                var r = x.Object;
                r.Id = x.Key;
                return r;
            }).Where(r => r.UserId == userId).ToList();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) => await GetRemindersByUserIdAsync(userId);

        public async Task SaveReminderAsync(Reminder reminder)
        {
            await _firebase.Child("Reminders").PostAsync(reminder);
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            if (!string.IsNullOrEmpty(reminder.Id))
            {
                await _firebase.Child("Reminders").Child(reminder.Id).PutAsync(reminder);
            }
        }

        public async Task<bool> DeleteReminderAsync(string id)
        {
            try
            {
                await _firebase.Child("Reminders").Child(id).DeleteAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task AddUserConnectionAsync(string familyId, string seniorId)
        {
            await _firebase.Child("UserConnections").PostAsync(new UserConnection { UserId = familyId, ConnectedUserId = seniorId });
        }

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId)
        {
            var connections = await _firebase.Child("UserConnections").OnceAsync<UserConnection>();
            return connections.Select(c => c.Object).Where(c => c.UserId == userId || c.ConnectedUserId == userId).ToList();
        }

        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            var connections = await GetUserConnectionsAsync(familyId);
            var elderIds = connections.Select(c => c.UserId == familyId ? c.ConnectedUserId : c.UserId).ToList();
            var allUsers = await GetUsersAsync();
            return allUsers.Where(u => elderIds.Contains(u.Id));
        }

        public async Task InviteElderAsync(string familyId, string elderId)
        {
            await _firebase.Child("PendingConnections").PostAsync(new PendingConnection { FamilyId = familyId, ElderId = elderId });
        }

        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId)
        {
            var data = await _firebase.Child("PendingConnections").OnceAsync<PendingConnection>();
            return data.Select(x => {
                var p = x.Object;
                p.Id = x.Key;
                return p;
            }).Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected);
        }

        public async Task ApproveConnectionAsync(PendingConnection request)
        {
            request.IsApproved = true;
            await _firebase.Child("PendingConnections").Child(request.Id).PutAsync(request);
            await AddUserConnectionAsync(request.FamilyId, request.ElderId);
        }

        public async Task RejectConnectionAsync(PendingConnection request)
        {
            request.IsRejected = true;
            await _firebase.Child("PendingConnections").Child(request.Id).PutAsync(request);
        }

        public async Task<List<UserRole>> GetRolesAsync() => new List<UserRole> { UserRole.Senior, UserRole.FamilyMember };
    }
}