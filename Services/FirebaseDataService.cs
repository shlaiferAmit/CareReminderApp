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

namespace CareReminderApp.Services
{
    public class FirebaseDataService : IDataService
    {
        private readonly FirebaseClient _firebase;
        private readonly AuthService _authService;
        private const string FirebaseUrl = "https://remaindsdb-default-rtdb.europe-west1.firebasedatabase.app";

        public FirebaseDataService(AuthService authService)
        {
            _firebase = new FirebaseClient(FirebaseUrl);
            _authService = authService;
        }

        public async Task<bool> RegisterUserAsync(string id, string firstName, string lastName, string email, string password, string mobile, UserRole role)
        {
            try
            {
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

                await _firebase
                    .Child("Users")
                    .Child(id) // משתמשים ב-ID של Firebase כקי (Key)
                    .PutAsync(newUser);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Error: {ex.Message}");
                return false;
            }
        }

        public async Task<User?> GetUserAsync(string email, string password)
        {
            try
            {
                var authUser = await _authService.SignInAsync(email, password);
                return await GetUserByIdAsync(authUser.User.Uid);
            }
            catch { return null; }
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _firebase.Child("Users").Child(id).OnceSingleAsync<User>();
                if (user != null) user.Id = id;
                return user;
            }
            catch { return null; }
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
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                await _firebase
                    .Child("Users")
                    .Child(user.Id) // או user.LocalId תלוי איך שמרת
                    .PutAsync(user); // Put מחליף את כל האובייקט, לכן ה-user חייב להכיל Role
                return true;
            }
            catch { return false; }
        }

        public async Task<string> UploadUserImageAsync(Stream imageStream, string userId)
        {
            var storage = new FirebaseStorage("care-reminder-app-amit.appspot.com");
            return await storage.Child("ProfileImages").Child($"{userId}.jpg").PutAsync(imageStream);
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId)
        {
            var data = await _firebase.Child("Reminders").OnceAsync<Reminder>();
            return data.Select(x => { var r = x.Object; r.Id = x.Key; return r; })
                       .Where(r => r.UserId == userId).ToList();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) => await GetRemindersByUserIdAsync(userId);

        public async Task SaveReminderAsync(Reminder reminder) => await _firebase.Child("Reminders").PostAsync(reminder);

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Id)) return;
            await _firebase.Child("Reminders").Child(reminder.Id).PutAsync(reminder);
        }

        public async Task<bool> DeleteReminderAsync(string reminderId)
        {
            try
            {
                // ודאי שהנתיב כאן תואם למבנה ה-Firebase שלך
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
            var ids = connections.Select(c => c.UserId == familyId ? c.ConnectedUserId : c.UserId);
            var allUsers = await GetUsersAsync();
            return allUsers.Where(u => ids.Contains(u.Id));
        }

        public async Task InviteElderAsync(string familyId, string elderId)
        {
            await _firebase.Child("PendingConnections").PostAsync(new PendingConnection { FamilyId = familyId, ElderId = elderId });
        }

        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId)
        {
            var data = await _firebase.Child("PendingConnections").OnceAsync<PendingConnection>();
            return data.Select(x => { var p = x.Object; p.Id = x.Key; return p; })
                       .Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected);
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