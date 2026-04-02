using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty; // שימי לב לשם המדויק
        public string UserPassword { get; set; } = string.Empty; // שימי לב לשם המדויק
        public string Mobile { get; set; } = string.Empty;
        public int RoleId { get; set; } // חייב להיות int כדי לפתור את שגיאת האופרטור
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Senior,
        FamilyMember
    }
}