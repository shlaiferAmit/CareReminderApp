using CareReminderApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace CareReminderApp.Services
{
    public interface IDataService
    {
        // ניהול משתמשים
        Task<User> GetUserAsync(string email, string password);
        Task<bool> RegisterUserAsync(string id, string firstName, string lastName, string email, string password, string mobile, UserRole role); Task<List<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(string id);

        // תזכורות
        Task<List<Reminder>> GetRemindersByUserIdAsync(string userId);
        Task<IEnumerable<Reminder>> GetRemindersAsync(string userId);
        Task SaveReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(string id);

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
        Task<bool> UpdateUserAsync(User user);
        Task<string> UploadUserImageAsync(Stream imageStream, string userId);
    }
}