using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareReminderApp.Services
{
    public interface IDataService
    {
        Task<User> GetUserAsync(string userEmail, string password);

        // כאן הוספנו את ה-mobile בסוף
        Task<bool> RegisterUserAsync(string firstName, string lastName, string userEmail, string password, string mobile, UserRole role);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<List<Reminder>> GetRemindersByUserIdAsync(string userId);
        Task AddReminderAsync(Reminder reminder);
        Task<List<UserConnection>> GetUserConnectionsAsync(string userId);
        Task<List<UserRole>> GetRolesAsync();
        Task<IEnumerable<Reminder>> GetRemindersAsync(string userId);
        Task UpdateReminderAsync(Reminder reminder);
    }
}