using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.Services
{
    public class MockDataService : IDataService
    {
        private List<User> _users = new List<User>();
        private List<Reminder> _reminders = new List<Reminder>();
        private List<UserConnection> _connections = new List<UserConnection>();

        public MockDataService()
        {
            // 1. הגדרת המשתמשים - שימי לב ל-IDs
            _users.Add(new User { Id = "1", FirstName = "Amit", UserEmail = "family@test.com", UserPassword = "123", Role = UserRole.FamilyMember });
            _users.Add(new User { Id = "2", FirstName = "Saba", UserEmail = "senior@test.com", UserPassword = "123", Role = UserRole.Senior });
            _users.Add(new User { Id = "101", FirstName = "Esther", UserEmail = "esther@test.com", UserPassword = "123", Role = UserRole.Senior });

            // 2. הגדרת הקשרים - שימוש בשמות השדות המדויקים מהמודל שלך (UserId ו-ConnectedUserId)
            _connections.Add(new UserConnection { UserId = "1", ConnectedUserId = "101" });

            // 3. תזכורת לדוגמה עבור אסתר
            _reminders.Add(new Reminder { Id = "1", Title = "Doctor appointment", Time = DateTime.Now.AddHours(1), IsCompleted = false, UserId = "101" });
        }

        // --- מימוש פונקציות ה-Interface ---

        public async Task<User> GetUserAsync(string userEmail, string password) =>
            await Task.FromResult(_users.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == password));

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile, UserRole role)
        {
            if (_users.Any(u => u.UserEmail == userEmail)) return false;
            _users.Add(new User { Id = Guid.NewGuid().ToString(), FirstName = firstName, LastName = lastName, UserEmail = userEmail, UserPassword = password, Mobile = mobile, Role = role });
            return true;
        }

        public async Task<List<User>> GetUsersAsync() => await Task.FromResult(_users.ToList());

        public async Task<User> GetUserByIdAsync(string id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) =>
            await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());

        public async Task AddReminderAsync(Reminder reminder)
        {
            reminder.Id = Guid.NewGuid().ToString();
            _reminders.Add(reminder);
            await Task.CompletedTask;
        }

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId) =>
            await Task.FromResult(_connections.Where(c => c.UserId == userId || c.ConnectedUserId == userId).ToList());

        public async Task<List<UserRole>> GetRolesAsync() =>
            await Task.FromResult(new List<UserRole> { UserRole.Senior, UserRole.FamilyMember });

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) =>
            await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            var existing = _reminders.FirstOrDefault(r => r.Id == reminder.Id);
            if (existing != null) existing.IsCompleted = reminder.IsCompleted;
            await Task.CompletedTask;
        }

        // הפונקציה הקריטית שמסננת את עמית מהרשימה
        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            // מוצאים את ה-IDs של המבוגרים המחוברים לפי השדות הנכונים
            var connectedIds = _connections
                .Where(c => c.UserId == familyId)
                .Select(c => c.ConnectedUserId)
                .ToList();

            // מחזירים רק את המשתמשים שה-ID שלהם ברשימת המחוברים
            return await Task.FromResult(_users.Where(u => connectedIds.Contains(u.Id)).ToList());
        }
    }
}