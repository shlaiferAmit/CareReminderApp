using Firebase.Database;
using Firebase.Database.Query;
using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.Services
{
    public class FirebaseDataService : IDataService
    {
        private readonly FirebaseClient _firebase;
        // כאן שמתי את הכתובת המדויקת מהתמונה שלך
        private const string FirebaseUrl = "https://remaindsdb-default-rtdb.europe-west1.firebasedatabase.app/";

        public FirebaseDataService()
        {
            _firebase = new FirebaseClient(FirebaseUrl);
        }

        public async Task<User> GetUserAsync(string userEmail, string password)
        {
            var users = await _firebase
                .Child("Users")
                .OnceAsync<User>();

            return users.Select(u => u.Object).FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == password);
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile, UserRole role)
        {
            try
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = firstName,
                    LastName = lastName,
                    UserEmail = userEmail,
                    UserPassword = password,
                    Mobile = mobile,
                    Role = role
                };

                // זה שומר את המשתמש בענן תחת תיקיית "Users"
                await _firebase.Child("Users").PostAsync(newUser);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // מימוש שאר הפונקציות מה-Interface
        public async Task<List<User>> GetUsersAsync() => (await _firebase.Child("Users").OnceAsync<User>()).Select(u => u.Object).ToList();

        public async Task AddReminderAsync(Reminder reminder) => await _firebase.Child("Reminders").PostAsync(reminder);

        // ... שאר הפונקציות ימומשו בהמשך בצורה דומה
        public Task<User> GetUserByIdAsync(string id) => throw new NotImplementedException();
        public Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) => throw new NotImplementedException();
        public Task<List<UserConnection>> GetUserConnectionsAsync(string userId) => throw new NotImplementedException();
        public Task<List<UserRole>> GetRolesAsync() => throw new NotImplementedException();
        public Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) => throw new NotImplementedException();
        public Task UpdateReminderAsync(Reminder reminder) => throw new NotImplementedException();
        public Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId) => throw new NotImplementedException();
    }
}