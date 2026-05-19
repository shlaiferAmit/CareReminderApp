using System;

namespace CareReminderApp.Models
{
    public class UserConnection
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ConnectedUserId { get; set; }

        public string CreatedAt { get; set; } = DateTime.Now.ToString("o");
    }
}