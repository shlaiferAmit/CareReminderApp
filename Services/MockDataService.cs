using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareReminderApp.Models;
using CareReminderApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Linq;

namespace CareReminderApp.Services
{
    public class MockDataService : IDataService
    {
        private List<User> _users = new List<User>
        {
            new User { Id = "1", FirstName = "Amit", UserEmail = "a@a.com", UserPassword = "123" }
        };

        public async Task<User> GetUserAsync(string userEmail, string password)
        {
            await Task.Delay(100);
            return _users.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == password);
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile)
        {
            if (_users.Any(u => u.UserEmail == userEmail)) return false;

            _users.Add(new User
            {
                Id = (_users.Count + 1).ToString(),
                FirstName = firstName,
                LastName = lastName,
                UserEmail = userEmail,
                UserPassword = password,
                Mobile = mobile
            });
            return await Task.FromResult(true);
        }

        // יתר המתודות (GetUsersAsync וכו') נשארות ללא שינוי
        public async Task<List<User>> GetUsersAsync() => await Task.FromResult(_users);
        public async Task<User> GetUserByIdAsync(string id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) => await Task.FromResult(new List<Reminder>());
        public async Task AddReminderAsync(Reminder reminder) => await Task.CompletedTask;
        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId) => await Task.FromResult(new List<UserConnection>());
        public async Task<List<UserRole>> GetRolesAsync() => await Task.FromResult(new List<UserRole>());
    }
}