using CareReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace CareReminderApp.Services
{
    public class MockDataService : IDataService
    {
        private List<User> _users = new List<User>();
        private List<Reminder> _reminders = new List<Reminder>();
        private List<UserConnection> _connections = new List<UserConnection>();
        private List<PendingConnection> _pendingConnections = new List<PendingConnection>();

        public MockDataService()
        {
            _users.Add(new User { Id = "1", FirstName = "Amit", UserEmail = "amit@test.com", Role = UserRole.FamilyMember });
            _users.Add(new User { Id = "101", FirstName = "Esther", UserEmail = "esther@test.com", Role = UserRole.Senior });
            _connections.Add(new UserConnection { UserId = "1", ConnectedUserId = "101" });
        }

        public async Task<bool> RegisterUserAsync(string id, string firstName, string lastName, string email, string password, string mobile, UserRole role)
        {
            // המימוש כאן לא באמת משנה כי את משתמשת ב-Firebase, אבל הוא חייב להיות קיים
            await Task.Delay(10);
            return true;
        }

        public async Task<User?> GetUserAsync(string email, string password)
        {
            return new User { UserEmail = email, FirstName = "Mock" };
        }

        public async Task<List<User>> GetUsersAsync() => await Task.FromResult(_users.ToList());
        public async Task<User?> GetUserByIdAsync(string id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) => await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());
        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) => await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());

        public async Task SaveReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Id)) reminder.Id = Guid.NewGuid().ToString();
            _reminders.Add(reminder);
            await Task.CompletedTask;
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            var existing = _reminders.FirstOrDefault(r => r.Id == reminder.Id);
            if (existing != null)
            {
                existing.IsCompleted = reminder.IsCompleted;
                existing.Title = reminder.Title;
                existing.DueDate = reminder.DueDate;
            }
            await Task.CompletedTask;
        }

        public async Task<bool> DeleteReminderAsync(string id)
        {
            var reminder = _reminders.FirstOrDefault(r => r.Id == id);
            if (reminder != null) { _reminders.Remove(reminder); return true; }
            return false;
        }

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId) =>
            await Task.FromResult(_connections.Where(c => c.UserId == userId || c.ConnectedUserId == userId).ToList());

        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            var ids = _connections.Where(c => c.UserId == familyId).Select(c => c.ConnectedUserId);
            return await Task.FromResult(_users.Where(u => ids.Contains(u.Id)));
        }

        public async Task AddUserConnectionAsync(string familyId, string seniorId)
        {
            if (!_connections.Any(c => c.UserId == familyId && c.ConnectedUserId == seniorId))
                _connections.Add(new UserConnection { UserId = familyId, ConnectedUserId = seniorId });
            await Task.CompletedTask;
        }

        public async Task InviteElderAsync(string familyId, string elderId)
        {
            _pendingConnections.Add(new PendingConnection { Id = Guid.NewGuid().ToString(), FamilyId = familyId, ElderId = elderId });
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId) =>
            await Task.FromResult(_pendingConnections.Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected));

        public async Task ApproveConnectionAsync(PendingConnection request)
        {
            var p = _pendingConnections.FirstOrDefault(x => x.Id == request.Id);
            if (p != null) { p.IsApproved = true; await AddUserConnectionAsync(p.FamilyId, p.ElderId); }
        }

        public async Task RejectConnectionAsync(PendingConnection request)
        {
            var p = _pendingConnections.FirstOrDefault(x => x.Id == request.Id);
            if (p != null) p.IsRejected = true;
        }

        public async Task<List<UserRole>> GetRolesAsync() => new List<UserRole> { UserRole.Senior, UserRole.FamilyMember };

        public async Task<bool> UpdateUserAsync(User user)
        {
            var ex = _users.FirstOrDefault(u => u.Id == user.Id);
            if (ex == null) return false;
            _users.Remove(ex); _users.Add(user);
            return true;
        }

        public async Task<string> UploadUserImageAsync(Stream imageStream, string userId) => "https://placeholder.com/user.jpg";
    }
}