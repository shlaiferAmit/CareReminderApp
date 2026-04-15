using CareReminderApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareReminderApp.Services
{
    public interface IDataService
    {
        // ניהול משתמשים
        Task<User?> GetUserAsync(string email, string password);
        Task<bool> RegisterUserAsync(string firstName, string lastName, string email, string password, string mobile, UserRole role);
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> FindSeniorByEmailAsync(string email);

        // תזכורות
        Task<List<Reminder>> GetRemindersByUserIdAsync(string userId);
        Task<IEnumerable<Reminder>> GetRemindersAsync(string userId);
        Task SaveReminderAsync(Reminder reminder);

        // קשרים
        Task<List<UserConnection>> GetUserConnectionsAsync(string userId);
        Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId);
        Task AddUserConnectionAsync(string familyId, string seniorId);

        // בקשות אישור
        Task InviteElderAsync(string familyId, string elderId);
        Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId);
        Task ApproveConnectionAsync(PendingConnection request);
        Task RejectConnectionAsync(PendingConnection request);

        Task<List<UserRole>> GetRolesAsync();
        Task UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(string id);
    }
}