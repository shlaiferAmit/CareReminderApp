using CareReminderApp.Models;
using CareReminderApp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CareReminderApp.Services
{
    public class MockDataService : IDataService
    {
        // רשימות שנשמרות בזיכרון לאורך כל זמן ריצת האפליקציה
        private List<User> _users = new List<User>();
        private List<Reminder> _reminders = new List<Reminder>();

        public MockDataService()
        {
            // אתחול משתמשים
            _users.Add(new User { Id = "1", FirstName = "Amit", UserEmail = "family@test.com", UserPassword = "123", Role = UserRole.FamilyMember });
            _users.Add(new User { Id = "2", FirstName = "Saba", UserEmail = "senior@test.com", UserPassword = "123", Role = UserRole.Senior });

            // אתחול תזכורות ראשוניות עבור סבא (Id = "2")
            _reminders.Add(new Reminder { Id = "1", Title = "מדידת לחץ דם", Time = DateTime.Now.AddHours(1), IsCompleted = false, UserId = "2" });
            _reminders.Add(new Reminder { Id = "2", Title = "כדור בוקר", Time = DateTime.Now, IsCompleted = true, UserId = "2" });
        }

        public async Task<User> GetUserAsync(string userEmail, string password)
        {
            await Task.Delay(100);
            return _users.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == password);
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile, UserRole role)
        {
            if (_users.Any(u => u.UserEmail == userEmail)) return false;

            _users.Add(new User
            {
                Id = (_users.Count + 1).ToString(),
                FirstName = firstName,
                LastName = lastName,
                UserEmail = userEmail,
                UserPassword = password,
                Mobile = mobile,
                Role = role
            });

            return await Task.FromResult(true);
        }

        public async Task<List<User>> GetUsersAsync() => await Task.FromResult(_users);

        public async Task<User> GetUserByIdAsync(string id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId)
        {
            await Task.Delay(100);
            // מחזיר את התזכורות מהרשימה המשותפת שבזיכרון
            return _reminders.Where(r => r.UserId == userId).ToList();
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            await Task.Delay(100);
            // מציאת התזכורת המקורית ברשימה ועדכון המצב שלה
            var existing = _reminders.FirstOrDefault(r => r.Id == reminder.Id);
            if (existing != null)
            {
                existing.IsCompleted = reminder.IsCompleted;
            }
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            await Task.Delay(100);
            reminder.Id = (_reminders.Count + 1).ToString();
            _reminders.Add(reminder);
        }

        public async Task<List<UserRole>> GetRolesAsync() =>
            await Task.FromResult(new List<UserRole> { UserRole.Senior, UserRole.FamilyMember });

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId) =>
            await Task.FromResult(new List<UserConnection>());

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) =>
            _reminders.Where(r => r.UserId == userId).ToList();
    }
}