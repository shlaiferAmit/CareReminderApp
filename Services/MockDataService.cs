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
        private List<PendingConnection> _pendingConnections = new List<PendingConnection>();

        public MockDataService()
        {
            _users.Add(new User { Id = "1", FirstName = "Amit", UserEmail = "amit@test.com", Role = UserRole.FamilyMember });
            _users.Add(new User { Id = "101", FirstName = "Esther", UserEmail = "esther@test.com", Role = UserRole.Senior });
            _connections.Add(new UserConnection { UserId = "1", ConnectedUserId = "101" });

            _reminders.Add(new Reminder
            {
                Id = "1",
                Title = "Medicine",
                DueDate = DateTime.Now.AddHours(2),
                UserId = "101"
            });
        }

        // --- תיקון שמות הפרמטרים כדי שיתאימו ל-Interface ---

        public async Task<User?> GetUserAsync(string email, string password) =>
            await Task.FromResult(_users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == password));

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string email, string password, string mobile, UserRole role)
        {
            if (_users.Any(u => u.UserEmail == email)) return false;
            _users.Add(new User { Id = Guid.NewGuid().ToString(), FirstName = firstName, LastName = lastName, UserEmail = email, UserPassword = password, Mobile = mobile, Role = role });
            return await Task.FromResult(true);
        }

        public async Task<List<User>> GetUsersAsync() => await Task.FromResult(_users.ToList());

        public async Task<User?> GetUserByIdAsync(string id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId) =>
            await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId) =>
            await Task.FromResult(_connections.Where(c => c.UserId == userId || c.ConnectedUserId == userId).ToList());

        public async Task<List<UserRole>> GetRolesAsync() =>
            await Task.FromResult(new List<UserRole> { UserRole.Senior, UserRole.FamilyMember });

        public async Task<IEnumerable<Reminder>> GetRemindersAsync(string userId) =>
            await Task.FromResult(_reminders.Where(r => r.UserId == userId).ToList());

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

        public async Task<IEnumerable<User>> GetEldersForFamilyAsync(string familyId)
        {
            var connectedIds = _connections
                .Where(c => c.UserId == familyId)
                .Select(c => c.ConnectedUserId)
                .ToList();

            var result = _users
                .Where(u => connectedIds.Contains(u.Id))
                .ToList();

            return await Task.FromResult(result);
        }

        public async Task<User?> FindSeniorByEmailAsync(string email) =>
            await Task.FromResult(_users.FirstOrDefault(u => u.UserEmail?.ToLower() == email.ToLower() && u.Role == UserRole.Senior));

        public async Task AddUserConnectionAsync(string familyId, string seniorId)
        {
            if (!_connections.Any(c => c.UserId == familyId && c.ConnectedUserId == seniorId))
            {
                _connections.Add(new UserConnection { UserId = familyId, ConnectedUserId = seniorId });
            }
            await Task.CompletedTask;
        }

        public async Task InviteElderAsync(string familyId, string elderId)
        {
            _pendingConnections.Add(new PendingConnection
            {
                Id = Guid.NewGuid().ToString(),
                FamilyId = familyId,
                ElderId = elderId,
                IsApproved = false,
                IsRejected = false
            });
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<PendingConnection>> GetPendingForElderAsync(string elderId)
        {
            return await Task.FromResult(_pendingConnections.Where(x => x.ElderId == elderId && !x.IsApproved && !x.IsRejected));
        }

        public async Task ApproveConnectionAsync(PendingConnection request)
        {
            var pending = _pendingConnections.FirstOrDefault(p => p.Id == request.Id);
            if (pending != null)
            {
                pending.IsApproved = true;
                await AddUserConnectionAsync(pending.FamilyId, pending.ElderId);
            }
            await Task.CompletedTask;
        }

        public async Task RejectConnectionAsync(PendingConnection request)
        {
            var pending = _pendingConnections.FirstOrDefault(p => p.Id == request.Id);
            if (pending != null) pending.IsRejected = true;
            await Task.CompletedTask;
        }

        public async Task SaveReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Id))
                reminder.Id = Guid.NewGuid().ToString();

            _reminders.Add(reminder);
            await Task.CompletedTask;
        }

        public async Task<bool> DeleteReminderAsync(string id)
        {
            var reminder = _reminders.FirstOrDefault(r => r.Id == id);
            if (reminder != null)
            {
                _reminders.Remove(reminder);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}